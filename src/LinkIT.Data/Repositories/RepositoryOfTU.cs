using LinkIT.Data.DTO;
using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace LinkIT.Data.Repositories
{
	public abstract class Repository<TDto, TQuery> : IRepository<TDto, TQuery> 
		where TDto : Dto 
		where TQuery : Query
	{
		public const string ID_COLUMN = "Id";
		public const string DELETED_COLUMN = "Deleted";
		public const string CREATION_DATE_COLUMN = "CreationDate";
		public const string CREATED_BY_COLUMN = "CreatedBy";
		public const string MODIFICATION_DATE_COLUMN = "ModificationDate";
		public const string MODIFIED_BY_COLUMN = "ModifiedBy";

		public Repository(string connectionString, string tableName, bool hasSoftDelete = false)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
				throw new ArgumentNullException("connectionString");
			if (string.IsNullOrWhiteSpace(tableName))
				throw new ArgumentNullException("tableName");

			ConnectionString = connectionString;
			TableName = tableName;
			HasSoftDelete = hasSoftDelete;
		}

		private static void AddPaging(SqlParameterCollection @params, StringBuilder sb, PageInfo pageInfo)
		{
			sb.AppendLine($"ORDER BY [{pageInfo.OrderBy.Name}] {pageInfo.OrderBy.Order.ForSql()}");
			sb.AppendLine("OFFSET ((@PageNumber - 1) * @RowsPerPage) ROWS");
			sb.AppendLine("FETCH NEXT @RowsPerPage ROWS ONLY");

			@params.Add("@PageNumber", SqlDbType.Int).Value = pageInfo.PageNumber;
			@params.Add("@RowsPerPage", SqlDbType.Int).Value = pageInfo.RowsPerPage;
		}

		private SqlCommand BuildSelectCountCommand(
			SqlConnection con,
			SqlTransaction tx,
			long[] ids)
		{
			var cmd = new SqlCommand { Connection = con, Transaction = tx };

			var sb = new StringBuilder();
			sb.AppendLine(CreateSelectCountStatement());

			var builder = new WhereInClauseBuilder(ID_COLUMN, cmd.Parameters, HasSoftDelete);
			builder.AddParameters(ids, SqlDbType.BigInt);
			sb.Append(builder);

			cmd.CommandText = sb.ToString();

			return cmd;
		}

		private SqlCommand BuildSelectCountCommand(
			SqlConnection con,
			SqlTransaction tx,
			TQuery query = null)
		{
			var cmd = new SqlCommand { Connection = con, Transaction = tx };

			var sb = new StringBuilder();
			sb.AppendLine(CreateSelectCountStatement());

			if (query != null)
			{
				var builder = new WhereClauseBuilder(cmd.Parameters, query.LogicalOperator, HasSoftDelete);
				BuildParametersFrom(query, builder);

				sb.Append(builder);
			}

			cmd.CommandText = sb.ToString();

			return cmd;
		}

		private SqlCommand BuildSelectCommand(
			SqlConnection con,
			SqlTransaction tx,
			long[] ids)
		{
			var cmd = new SqlCommand { Connection = con, Transaction = tx };

			var sb = new StringBuilder();
			sb.AppendLine(CreateSelectStatement());

			var builder = new WhereInClauseBuilder(ID_COLUMN, cmd.Parameters, HasSoftDelete);
			builder.AddParameters(ids, SqlDbType.BigInt);
			sb.Append(builder);

			cmd.CommandText = sb.ToString();

			return cmd;
		}

		private SqlCommand BuildSelectCommand(
			SqlConnection con,
			SqlTransaction tx,
			TQuery query = null,
			PageInfo pageInfo = null)
		{
			var cmd = new SqlCommand { Connection = con, Transaction = tx };

			var sb = new StringBuilder();
			sb.AppendLine(CreateSelectStatement());

			if (query != null)
			{
				var builder = new WhereClauseBuilder(cmd.Parameters, query.LogicalOperator, HasSoftDelete);
				BuildParametersFrom(query, builder);

				sb.Append(builder);
			}

			if (pageInfo != null)
				AddPaging(cmd.Parameters, sb, pageInfo);

			cmd.CommandText = sb.ToString();

			return cmd;
		}

		protected static T GetColumnValue<T>(SqlDataReader reader, string columnName)
		{
			object value = reader[columnName];
			if (value == null || value == DBNull.Value)
				return default;

			return (T)value;
		}

		protected abstract void BuildParametersFrom(TDto input, SqlParameterBuilder builder);

		protected abstract void BuildParametersFrom(TQuery input, WhereClauseBuilder builder);

		protected abstract IEnumerable<TDto> ReadDtosFrom(SqlDataReader reader);

		protected abstract string CreateInsertStatement();

		protected abstract string CreateUpdateStatement();

		protected string ConnectionString { get; }

		protected string TableName { get; }

		protected bool HasSoftDelete { get; }

		protected string CreateSelectStatement() => $"SELECT * FROM [{TableName}]";

		protected string CreateSelectCountStatement() => $"SELECT COUNT({ID_COLUMN}) FROM [{TableName}]";

		public abstract IEnumerable<string> Columns { get; }

		public bool Exists(long id) => Exists(new[] { id });

		public bool Exists(IEnumerable<long> ids)
		{
			if (ids == null || !ids.Any())
				throw new ArgumentNullException("ids");

			// Filter out possible duplicates.
			var distinctIds = ids.Distinct().ToArray();

			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = BuildSelectCountCommand(con, tx, distinctIds))
					{
						long count = Convert.ToInt64(cmd.ExecuteScalar());

						return distinctIds.Length == count;
					}

					//tx.Commit();
				}
			}
		}

		public TDto GetById(long id) => GetById(new[] { id }).Single();

		public virtual IEnumerable<TDto> GetById(IEnumerable<long> ids)
		{
			if (ids == null || !ids.Any())
				throw new ArgumentNullException("ids");

			// Filter out possible duplicates.
			var distinctIds = ids.Distinct().ToArray();

			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = BuildSelectCountCommand(con, tx, distinctIds))
					{
						long count = Convert.ToInt64(cmd.ExecuteScalar());
						if (distinctIds.Length != count)
							throw new ArgumentException("Not all supplied id's exist.");
					}

					using (var cmd = BuildSelectCommand(con, tx, distinctIds))
					using (var reader = cmd.ExecuteReader())
					{
						return ReadDtosFrom(reader).ToList();
					}

					//tx.Commit();
				}
			}
		}

		public virtual IEnumerable<TDto> Query(TQuery query = null)
		{
			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = BuildSelectCommand(con, tx, query))
					using (var reader = cmd.ExecuteReader())
					{
						return ReadDtosFrom(reader).ToList();
					}

					//tx.Commit();
				}
			}
		}

		public virtual PagedResult<TDto> PagedQuery(PageInfo pageInfo, TQuery query = null)
		{
			if (pageInfo == null)
				throw new ArgumentNullException("pageInfo");

			if (!pageInfo.OrderBy.IsValidFor(Columns))
				throw new ArgumentException($"'{pageInfo.OrderBy.Name}' is an unrecognized column.");

			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					long totalCount;
					using (var cmd = BuildSelectCountCommand(con, tx, query))
					{
						totalCount = Convert.ToInt64(cmd.ExecuteScalar());
					}

					using (var cmd = BuildSelectCommand(con, tx, query, pageInfo))
					using (var reader = cmd.ExecuteReader())
					{
						var result = ReadDtosFrom(reader).ToList();

						return new PagedResult<TDto>(result, pageInfo, totalCount);
					}

					//tx.Commit();
				}
			}
		}

		public virtual long Insert(TDto item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			if (item.Id.HasValue)
				throw new ArgumentException("Id can not be specified.");

			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText = CreateInsertStatement();
					cmdText += "\nSELECT CONVERT(bigint, SCOPE_IDENTITY())";

					long newId;
					using (var cmd = new SqlCommand(cmdText, con, tx))
					{
						var builder = new SqlParameterBuilder(cmd.Parameters);
						BuildParametersFrom(item, builder);

						newId = (long)cmd.ExecuteScalar();
					}

					tx.Commit();

					return newId;
				}
			}
		}

		public void Update(TDto item) => Update(new[] { item });

		public virtual void Update(IEnumerable<TDto> items)
		{
			if (items == null || !items.Any())
				throw new ArgumentNullException("items");

			foreach (var item in items)
			{
				if (!item.Id.HasValue)
					throw new ArgumentException("Id is a required field.");
			}

			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText = CreateUpdateStatement();
					foreach (var item in items)
					{
						using (var cmd = new SqlCommand(cmdText, con, tx))
						{
							var builder = new SqlParameterBuilder(cmd.Parameters);
							BuildParametersFrom(item, builder);

							cmd.ExecuteNonQuery();
						}
					}

					tx.Commit();
				}
			}
		}

		public void Delete(long id)
		{
			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText;
					if (HasSoftDelete)
					{
						cmdText = $"UPDATE [{TableName}] SET {DELETED_COLUMN} = 1 WHERE [{ID_COLUMN}]=@{ID_COLUMN}";
					}
					else
					{
						cmdText = $"DELETE FROM [{TableName}] WHERE [{ID_COLUMN}]=@{ID_COLUMN}";
					}

					using (var cmd = new SqlCommand(cmdText, con, tx))
					{
						cmd.Parameters.Add($"@{ID_COLUMN}", SqlDbType.BigInt).Value = id;

						cmd.ExecuteNonQuery();
					}

					tx.Commit();
				}
			}
		}
	}
}