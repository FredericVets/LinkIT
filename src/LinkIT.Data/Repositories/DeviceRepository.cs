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
			if (string.IsNullOrEmpty(connectionString))
				throw new ArgumentNullException();

			_connectionString = connectionString;
		}

		/// <summary>
		/// This will build the SqlCommand based on the query object and the specified condition (AND / OR to combine the arguments).
		/// Has support for paging.
		/// This is based on the new paging feature introduced in Sql Serever 2012.
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

		public DeviceDto Get(long id)
		{
			var query = new DeviceQuery { Id = id };
			var result = Query(query);

			if (result.Count() == 0)
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

		public long Insert(DeviceDto input)
		{
			input.ValidateRequiredFields(forInsert: true);

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

					using (var cmd = new SqlCommand(cmdText, con, tx))
					{
						cmd.Parameters.Add("@Tag", SqlDbType.NVarChar).Value = input.Tag;
						cmd.Parameters.Add("@Owner", SqlDbType.NVarChar).Value = input.Owner;
						cmd.Parameters.Add("@Brand", SqlDbType.NVarChar).Value = input.Brand;
						cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = input.Type;

						long newId = (long)cmd.ExecuteScalar();
						input.Id = newId;
					}

					tx.Commit();

					return input.Id.Value;
				}
			}
		}

		/// <summary>
		/// This is a full-update. So all required fields should be supplied.
		/// </summary>
		/// <param name="input"></param>
		public void Update(DeviceDto input)
		{
			input.ValidateRequiredFields();

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

					using (var cmd = new SqlCommand(cmdText, con, tx))
					{
						cmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = input.Id.Value;
						cmd.Parameters.Add("@Tag", SqlDbType.NVarChar).Value = input.Tag;
						cmd.Parameters.Add("@Owner", SqlDbType.NVarChar).Value = input.Owner;
						cmd.Parameters.Add("@Brand", SqlDbType.NVarChar).Value = input.Brand;
						cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = input.Type;

						cmd.ExecuteNonQuery();
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