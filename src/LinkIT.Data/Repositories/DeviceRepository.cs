using LinkIT.Data.DTO;
using LinkIT.Data.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace LinkIT.Data.Repositories
{
	public class DeviceRepository : IRepository<DeviceDto, DeviceQuery>
	{
		public const string ID_COLUMN = "Id";
		public const string TAG_COLUMN = "Tag";
		public const string OWNER_COLUMN = "Owner";
		public const string BRAND_COLUMN = "Brand";
		public const string TYPE_COLUMN = "Type";

		private string _connectionString;

		public DeviceRepository(string connectionString)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
				throw new ArgumentNullException("connectionString");

			_connectionString = connectionString;
		}

		/// <summary>
		/// This will build the SqlCommand based on the optional query object and the optional condition (AND / OR to combine 
		/// the query arguments).
		/// Has support for paging. This is based on the new paging feature introduced in Sql Serever 2012.
		/// If no query or paging instance is supplied, a select without where clause will be generated.
		/// <see cref="https://social.technet.microsoft.com/wiki/contents/articles/23811.paging-a-query-with-sql-server.aspx#Paginacao_dentro"/>
		/// </summary>
		/// <param name="con"></param>
		/// <param name="tx"></param>
		/// <param name="query"></param>
		/// <param name="condition"></param>
		/// <param name="paging"></param>
		/// <returns></returns>
		private static SqlCommand CreateSelectCommand(
			SqlConnection con,
			SqlTransaction tx,
			DeviceQuery query = null,
			WhereCondition condition = WhereCondition.AND,
			Paging paging = null)
		{
			var cmd = new SqlCommand { Connection = con, Transaction = tx };

			var sb = new StringBuilder();
			sb.AppendFormat("SELECT * FROM [{0}]", TableNames.DEVICE_TABLE);
			sb.AppendLine();

			if (query != null)
				AddWhereClause(cmd.Parameters, sb, query, condition);

			if (paging != null)
				AddPaging(cmd.Parameters, sb, paging);

			cmd.CommandText = sb.ToString();

			return cmd;
		}

		private static SqlCommand CreateSelectCountCommand(
			SqlConnection con,
			SqlTransaction tx,
			DeviceQuery query = null)
		{
			var cmd = new SqlCommand { Connection = con, Transaction = tx };
			
			var sb = new StringBuilder();
			sb.AppendFormat("SELECT COUNT(*) FROM [{0}]", TableNames.DEVICE_TABLE);
			sb.AppendLine();

			if (query != null)
				AddWhereClause(cmd.Parameters, sb, query, WhereCondition.AND);

			cmd.CommandText = sb.ToString();

			return cmd;
		}

		private static void AddWhereClause(SqlParameterCollection @params, StringBuilder sb, DeviceQuery query, WhereCondition condition)
		{
			sb.AppendLine("WHERE");

			bool firstCondition = true;
			if (query.Id.HasValue)
			{
				sb.AppendFormat("[{0}] = @Id", ID_COLUMN);
				sb.AppendLine();
				@params.Add("@Id", SqlDbType.BigInt).Value = query.Id.Value;
				firstCondition = false;
			}

			if (!string.IsNullOrWhiteSpace(query.Tag))
			{
				if (!firstCondition)
					sb.AppendLine(condition.ToString());

				sb.AppendFormat("[{0}] = @Tag", TAG_COLUMN);
				sb.AppendLine();
				@params.Add("@Tag", SqlDbType.NVarChar).Value = query.Tag;
				firstCondition = false;
			}

			if (!string.IsNullOrWhiteSpace(query.Owner))
			{
				if (!firstCondition)
					sb.AppendLine(condition.ToString());

				sb.AppendFormat("[{0}] = @Owner", OWNER_COLUMN);
				sb.AppendLine();
				@params.Add("@Owner", SqlDbType.NVarChar).Value = query.Owner;
				firstCondition = false;
			}

			if (!string.IsNullOrWhiteSpace(query.Brand))
			{
				if (!firstCondition)
					sb.AppendLine(condition.ToString());

				sb.AppendFormat("[{0}] = @Brand", BRAND_COLUMN);
				sb.AppendLine();
				@params.Add("@Brand", SqlDbType.NVarChar).Value = query.Brand;
				firstCondition = false;
			}

			if (!string.IsNullOrWhiteSpace(query.Type))
			{
				if (!firstCondition)
					sb.AppendLine(condition.ToString());

				sb.AppendFormat("[{0}] = @Type", TYPE_COLUMN);
				sb.AppendLine();
				@params.Add("@Type", SqlDbType.NVarChar).Value = query.Type;
				firstCondition = false;
			}
		}

		private static void AddPaging(SqlParameterCollection @params, StringBuilder sb, Paging paging)
		{
			sb.AppendFormat("ORDER BY [{0}]", paging.OrderByColumnName);
			sb.AppendLine();
			sb.AppendLine("OFFSET ((@PageNumber - 1) * @RowsPerPage) ROWS");
			sb.AppendLine("FETCH NEXT @RowsPerPage ROWS ONLY");

			@params.Add("@PageNumber", SqlDbType.Int).Value = paging.PageNumber;
			@params.Add("@RowsPerPage", SqlDbType.Int).Value = paging.RowsPerPage;
		}

		public bool Exists(long id)
		{
			using (var con = new SqlConnection(_connectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = CreateSelectCountCommand(
						con,
						tx,
						new DeviceQuery { Id = id }))
					{
						int count = (int)cmd.ExecuteScalar();

						return count == 1;
					}

					//tx.Commit();
				}
			}
		}

		public DeviceDto Get(long id)
		{
			var query = new DeviceQuery { Id = id };
			var result = Query(query);

			if (result.Count() != 1)
				throw new InvalidOperationException(string.Format("No device found for id : '{0}'.", id));

			return result.Single();
		}

		public IEnumerable<DeviceDto> Query(
			DeviceQuery query = null,
			WhereCondition condition = WhereCondition.AND,
			Paging paging = null)
		{
			var result = new List<DeviceDto>();

			using (var con = new SqlConnection(_connectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = CreateSelectCommand(con, tx, query, condition, paging))
					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							result.Add(new DeviceDto
							{
								Id = (long)reader[ID_COLUMN],
								Tag = reader[TAG_COLUMN].ToString(),
								Owner = reader[OWNER_COLUMN].ToString(),
								Brand = reader[BRAND_COLUMN].ToString(),
								Type = reader[TYPE_COLUMN].ToString()
							});
						}

						return result;
					}

					//tx.Commit();
				}
			}
		}

		public long Insert(DeviceDto item)
		{
			item.ValidateRequiredFields(forInsert: true);

			using (var con = new SqlConnection(_connectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText = string.Format(
						@"INSERT into [{0}] ([{1}], [{2}], [{3}], [{4}]) VALUES (@Tag, @Owner, @Brand, @Type)
						SELECT CONVERT(bigint, SCOPE_IDENTITY())",
						TableNames.DEVICE_TABLE,
						TAG_COLUMN,
						OWNER_COLUMN,
						BRAND_COLUMN,
						TYPE_COLUMN);

					long newId;
					using (var cmd = new SqlCommand(cmdText, con, tx))
					{
						cmd.Parameters.Add("@Tag", SqlDbType.NVarChar).Value = item.Tag;
						cmd.Parameters.Add("@Owner", SqlDbType.NVarChar).Value = item.Owner;
						cmd.Parameters.Add("@Brand", SqlDbType.NVarChar).Value = item.Brand;
						cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = item.Type;

						newId = (long)cmd.ExecuteScalar();
					}

					tx.Commit();

					return newId;
				}
			}
		}

		/// <summary>
		/// This is a full-update. So all required fields should be supplied.
		/// </summary>
		/// <param name="item"></param>
		public void Update(DeviceDto item)
		{
			Update(new[] { item });
		}

		public void Update(IEnumerable<DeviceDto> data)
		{
			foreach (var item in data)
				item.ValidateRequiredFields();

			using (var con = new SqlConnection(_connectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText = string.Format(
						@"UPDATE [{0}] SET [{1}]=@Tag, [{2}]=@Owner, [{3}]=@Brand, [{4}]=@Type WHERE [{5}]=@Id",
						TableNames.DEVICE_TABLE,
						TAG_COLUMN,
						OWNER_COLUMN,
						BRAND_COLUMN,
						TYPE_COLUMN,
						ID_COLUMN);

					foreach (var item in data)
					{
						using (var cmd = new SqlCommand(cmdText, con, tx))
						{
							cmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = item.Id.Value;
							cmd.Parameters.Add("@Tag", SqlDbType.NVarChar).Value = item.Tag;
							cmd.Parameters.Add("@Owner", SqlDbType.NVarChar).Value = item.Owner;
							cmd.Parameters.Add("@Brand", SqlDbType.NVarChar).Value = item.Brand;
							cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = item.Type;

							cmd.ExecuteNonQuery();
						}
					}

					tx.Commit();
				}
			}
		}

		public void Delete(long id)
		{
			using (var con = new SqlConnection(_connectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText = string.Format(
						@"DELETE FROM [{0}] WHERE [{1}]=@Id",
						TableNames.DEVICE_TABLE,
						ID_COLUMN);

					using (var cmd = new SqlCommand(cmdText, con, tx))
					{
						cmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = id;

						cmd.ExecuteNonQuery();
					}

					tx.Commit();
				}
			}
		}
	}
}