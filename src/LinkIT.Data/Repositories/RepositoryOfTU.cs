﻿using LinkIT.Data.Builders;
using LinkIT.Data.DTO;
using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LinkIT.Data.Repositories
{
	// Select statements are also wrapped in transactions that are implicitly rollbacked.
	// So there is always a consistent view of the database inside the transaction.
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

		public Repository(ConnectionString connString, string tableName, bool hasSoftDelete = false)
		{
			if (string.IsNullOrWhiteSpace(tableName))
				throw new ArgumentNullException(nameof(tableName));

			ConnectionString = connString ?? throw new ArgumentNullException(nameof(connString));
			TableName = tableName;
			HasSoftDelete = hasSoftDelete;
		}

		protected abstract void AddParametersFor(TDto input, SqlParameterBuilder builder);

		protected abstract void AddParametersFor(TQuery input, WhereClauseBuilder builder);

		protected abstract IEnumerable<TDto> ReadDtosFrom(SqlDataReader reader);

		protected abstract string CreateInsertStatement();

		protected abstract string CreateUpdateStatement();

		protected ConnectionString ConnectionString { get; }

		protected string TableName { get; }

		protected bool HasSoftDelete { get; }

		protected string CreateSelectStatement() => 
			$"SELECT * FROM [{TableName}]";

		protected string CreateSelectCountStatement() => 
			$"SELECT COUNT({ID_COLUMN}) FROM [{TableName}]";

		public abstract IEnumerable<string> Columns { get; }

		public static T GetColumnValue<T>(SqlDataReader reader, string columnName)
		{
			object value = reader[columnName];
			if (value == null || value == DBNull.Value)
				return default;

			return (T)value;
		}

		public bool Exists(long id) => 
			Exists(new[] { id });

		public bool Exists(IEnumerable<long> ids)
		{
			if (ids == null || !ids.Any())
				throw new ArgumentNullException(nameof(ids));

			// Filter out possible duplicates.
			var distinctIds = ids.Distinct().ToArray();

			using (var con = new SqlConnection(ConnectionString.Value))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = new SelectCommandBuilder(con, tx, HasSoftDelete)
						.ForSelect(CreateSelectCountStatement())
						.ForWhereIn(ID_COLUMN, distinctIds, SqlDbType.BigInt)
						.Build())
					{
						long count = Convert.ToInt64(cmd.ExecuteScalar());

						return distinctIds.Length == count;
					}

					//tx.Commit();
				}
			}
		}

		public TDto GetById(long id) => 
			GetById(new[] { id }).Single();

		public virtual IEnumerable<TDto> GetById(IEnumerable<long> ids)
		{
			if (ids == null || !ids.Any())
				throw new ArgumentNullException(nameof(ids));

			// Filter out possible duplicates.
			var distinctIds = ids.Distinct().ToArray();

			using (var con = new SqlConnection(ConnectionString.Value))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = new SelectCommandBuilder(con, tx, HasSoftDelete)
						.ForSelect(CreateSelectCountStatement())
						.ForWhereIn(ID_COLUMN, distinctIds, SqlDbType.BigInt)
						.Build())
					{
						long count = Convert.ToInt64(cmd.ExecuteScalar());
						if (distinctIds.Length != count)
							throw new ArgumentException("Not all supplied id's exist.");
					}

					using (var cmd = new SelectCommandBuilder(con, tx, HasSoftDelete)
						.ForSelect(CreateSelectStatement())
						.ForWhereIn(ID_COLUMN, distinctIds, SqlDbType.BigInt)
						.Build())
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
			using (var con = new SqlConnection(ConnectionString.Value))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = new SelectCommandBuilder(con, tx, HasSoftDelete)
						.ForSelect(CreateSelectStatement())
						.ForWhere(query, AddParametersFor)
						.Build())
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
				throw new ArgumentNullException(nameof(pageInfo));

			if (!pageInfo.OrderBy.IsValidFor(Columns))
				throw new ArgumentException($"'{pageInfo.OrderBy.Name}' is an unrecognized column.");

			using (var con = new SqlConnection(ConnectionString.Value))
			{
				con.Open();

				// If you want to protect against phantom reads when data gets inserted inbetween the select statements,
				// the isolation level of the transaction should be set to 'IsolationLevel.Serializable'.
				// See link for more info :
				// https://stackoverflow.com/questions/3467613/isolation-level-serializable-when-should-i-use-this
				using (var tx = con.BeginTransaction())
				{
					long totalCount;
					using (var cmd = new SelectCommandBuilder(con, tx, HasSoftDelete)
						.ForSelect(CreateSelectCountStatement())
						.ForWhere(query, AddParametersFor)
						.Build())
					{
						totalCount = Convert.ToInt64(cmd.ExecuteScalar());
					}

					using (var cmd = new SelectCommandBuilder(con, tx, HasSoftDelete)
						.ForSelect(CreateSelectStatement())
						.ForWhere(query, AddParametersFor)
						.ForPaging(pageInfo)
						.Build())
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
				throw new ArgumentNullException(nameof(item));
			if (item.Id.HasValue)
				throw new ArgumentException("Id can not be specified.");

			using (var con = new SqlConnection(ConnectionString.Value))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText = CreateInsertStatement();
					cmdText += "\nSELECT CONVERT(bigint, SCOPE_IDENTITY())";

					long newId;
					using (var cmd = new SqlCommand(cmdText, con, tx))
					{
						var builder = new SqlParameterBuilder(cmd);
						AddParametersFor(item, builder);

						newId = (long)cmd.ExecuteScalar();
					}

					tx.Commit();

					return newId;
				}
			}
		}

		public void Update(TDto item) => 
			Update(new[] { item });

		public virtual void Update(IEnumerable<TDto> items)
		{
			if (items == null || !items.Any())
				throw new ArgumentNullException(nameof(items));

			foreach (var item in items)
			{
				if (!item.Id.HasValue)
					throw new ArgumentException("Id is a required field.");
			}

			using (var con = new SqlConnection(ConnectionString.Value))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText = CreateUpdateStatement();
					foreach (var item in items)
					{
						using (var cmd = new SqlCommand(cmdText, con, tx))
						{
							var builder = new SqlParameterBuilder(cmd);
							AddParametersFor(item, builder);

							cmd.ExecuteNonQuery();
						}
					}

					tx.Commit();
				}
			}
		}

		public virtual void Delete(long id)
		{
			using (var con = new SqlConnection(ConnectionString.Value))
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