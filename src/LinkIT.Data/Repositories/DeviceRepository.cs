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
	public class DeviceRepository : IDeviceRepository
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

		private static void AddSqlParametersTo(SqlCommand cmd, DeviceDto input)
		{
			if (input.Id.HasValue)
				cmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = input.Id.Value;

			cmd.Parameters.Add("@Tag", SqlDbType.NVarChar).Value = input.Tag;
			cmd.Parameters.Add("@Owner", SqlDbType.NVarChar).Value = input.Owner;
			cmd.Parameters.Add("@Brand", SqlDbType.NVarChar).Value = input.Brand;
			cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = input.Type;
		}

		private static IEnumerable<DeviceDto> ReadDtosFrom(SqlDataReader reader)
		{
			while (reader.Read())
			{
				yield return new DeviceDto
				{
					Id = (long)reader[ID_COLUMN],
					Tag = reader[TAG_COLUMN].ToString(),
					Owner = reader[OWNER_COLUMN].ToString(),
					Brand = reader[BRAND_COLUMN].ToString(),
					Type = reader[TYPE_COLUMN].ToString()
				};
			}
		}

		/// <summary>
		/// This will build the SqlCommand based on the optional query object. The specified Logical operator will be 
		/// used to combine the query arguments.
		/// Has support for paging. This is based on the new paging feature introduced in Sql Serever 2012.
		/// If no query or paging instance is supplied, a select without where clause will be generated.
		/// <see cref="https://social.technet.microsoft.com/wiki/contents/articles/23811.paging-a-query-with-sql-server.aspx#Paginacao_dentro"/>
		/// </summary>
		/// <param name="con"></param>
		/// <param name="tx"></param>
		/// <param name="query"></param>
		/// <param name="paging"></param>
		/// <returns></returns>
		private static SqlCommand CreateSelectCommandFor(
			SqlConnection con,
			SqlTransaction tx,
			DeviceQuery query = null,
			Paging paging = null)
		{
			var cmd = new SqlCommand { Connection = con, Transaction = tx };

			var sb = new StringBuilder();
			sb.AppendLine($"SELECT * FROM [{TableNames.DEVICE_TABLE}]");

			if (query != null)
				AddWhereClause(cmd.Parameters, sb, query);

			if (paging != null)
				AddPaging(cmd.Parameters, sb, paging);

			cmd.CommandText = sb.ToString();

			return cmd;
		}

		private static SqlCommand CreateSelectCountCommandFor(
			SqlConnection con,
			SqlTransaction tx,
			DeviceQuery query = null)
		{
			var cmd = new SqlCommand { Connection = con, Transaction = tx };

			var sb = new StringBuilder();
			sb.AppendLine($"SELECT COUNT({ID_COLUMN}) FROM [{TableNames.DEVICE_TABLE}]");

			if (query != null)
				AddWhereClause(cmd.Parameters, sb, query);

			cmd.CommandText = sb.ToString();

			return cmd;
		}

		private static SqlCommand CreateSelectWhereIdInCommand(
			SqlConnection con,
			SqlTransaction tx,
			IList<long> ids)
		{
			var cmd = new SqlCommand { Connection = con, Transaction = tx };

			var sb = new StringBuilder();
			sb.AppendLine($"SELECT * FROM {TableNames.DEVICE_TABLE}");
			sb.Append($"WHERE [{ID_COLUMN}] IN (");

			bool first = true;
			for (int i = 0; i < ids.Count; i++)
			{
				if (!first)
					sb.Append(", ");

				string identifier = $"@Id{i}";
				sb.Append(identifier);
				cmd.Parameters.Add(identifier, SqlDbType.BigInt).Value = ids[i];

				first = false;
			}

			sb.AppendLine(")");

			cmd.CommandText = sb.ToString();

			return cmd;
		}

		private static void AddWhereClause(SqlParameterCollection @params, StringBuilder sb, DeviceQuery query)
		{
			sb.AppendLine("WHERE");

			bool firstCondition = true;
			if (query.Id.HasValue)
			{
				sb.AppendLine($"[{ID_COLUMN}] = @Id");
				@params.Add("@Id", SqlDbType.BigInt).Value = query.Id.Value;
				firstCondition = false;
			}

			if (!string.IsNullOrWhiteSpace(query.Tag))
			{
				if (!firstCondition)
					sb.AppendLine(query.LogicalOperator.ToString());

				sb.AppendLine($"[{TAG_COLUMN}] = @Tag");
				@params.Add("@Tag", SqlDbType.NVarChar).Value = query.Tag;
				firstCondition = false;
			}

			if (!string.IsNullOrWhiteSpace(query.Owner))
			{
				if (!firstCondition)
					sb.AppendLine(query.LogicalOperator.ToString());

				sb.AppendLine($"[{OWNER_COLUMN}] = @Owner");
				@params.Add("@Owner", SqlDbType.NVarChar).Value = query.Owner;
				firstCondition = false;
			}

			if (!string.IsNullOrWhiteSpace(query.Brand))
			{
				if (!firstCondition)
					sb.AppendLine(query.LogicalOperator.ToString());

				sb.AppendLine($"[{BRAND_COLUMN}] = @Brand");
				@params.Add("@Brand", SqlDbType.NVarChar).Value = query.Brand;
				firstCondition = false;
			}

			if (!string.IsNullOrWhiteSpace(query.Type))
			{
				if (!firstCondition)
					sb.AppendLine(query.LogicalOperator.ToString());

				sb.AppendLine($"[{TYPE_COLUMN}] = @Type");
				@params.Add("@Type", SqlDbType.NVarChar).Value = query.Type;
				firstCondition = false;
			}
		}

		private static void AddPaging(SqlParameterCollection @params, StringBuilder sb, Paging paging)
		{
			sb.Append($"ORDER BY [{paging.OrderByColumnName}] ");
			if (paging.OrderBySorting == Sorting.ASCENDING)
			{
				sb.AppendLine("ASC");
			}
			else
			{
				sb.AppendLine("DESC");
			}

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
					using (var cmd = CreateSelectCountCommandFor(
						con,
						tx,
						new DeviceQuery { Id = id }))
					{
						long count = Convert.ToInt64(cmd.ExecuteScalar());

						return count == 1;
					}

					//tx.Commit();
				}
			}
		}

		public DeviceDto Get(long id)
		{
			var result = Get(new[] { id }).ToList();

			if (result.Count != 1)
				throw new InvalidOperationException($"No device found for id : '{id}'.");

			return result.Single();
		}

		public IEnumerable<DeviceDto> Get(IEnumerable<long> ids)
		{
			using (var con = new SqlConnection(_connectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = CreateSelectWhereIdInCommand(con, tx, ids.ToList()))
					using (var reader = cmd.ExecuteReader())
					{
						return ReadDtosFrom(reader).ToList();
					}

					//tx.Commit();
				}
			}
		}

		public IEnumerable<DeviceDto> Query(DeviceQuery query = null)
		{
			using (var con = new SqlConnection(_connectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = CreateSelectCommandFor(con, tx, query))
					using (var reader = cmd.ExecuteReader())
					{
						return ReadDtosFrom(reader).ToList();
					}

					//tx.Commit();
				}
			}
		}

		public PagedResult<DeviceDto> PagedQuery(Paging paging, DeviceQuery query = null)
		{
			using (var con = new SqlConnection(_connectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					long totalCount;
					using (var cmd = CreateSelectCountCommandFor(con, tx, query))
					{
						totalCount = Convert.ToInt64(cmd.ExecuteScalar());
					}

					using (var cmd = CreateSelectCommandFor(con, tx, query, paging))
					using (var reader = cmd.ExecuteReader())
					{
						var result = ReadDtosFrom(reader).ToList();

						return new PagedResult<DeviceDto>(result, paging, totalCount);
					}

					//tx.Commit();
				}
			}
		}

		public long Insert(DeviceDto item)
		{
			using (var con = new SqlConnection(_connectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText = $@"INSERT INTO [{TableNames.DEVICE_TABLE}] ([{TAG_COLUMN}], [{OWNER_COLUMN}], [{BRAND_COLUMN}], [{TYPE_COLUMN}]) 
						VALUES (@Tag, @Owner, @Brand, @Type)
						SELECT CONVERT(bigint, SCOPE_IDENTITY())";

					long newId;
					using (var cmd = new SqlCommand(cmdText, con, tx))
					{
						AddSqlParametersTo(cmd, item);

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
			{
				if (!item.Id.HasValue)
					throw new ArgumentException("Id is a required field.");
			}

			using (var con = new SqlConnection(_connectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText = $@"UPDATE [{TableNames.DEVICE_TABLE}] 
								SET [{TAG_COLUMN}]=@Tag, [{OWNER_COLUMN}]=@Owner, [{BRAND_COLUMN}]=@Brand, [{TYPE_COLUMN}]=@Type 
								WHERE [{ID_COLUMN}]=@Id";

					foreach (var item in data)
					{
						using (var cmd = new SqlCommand(cmdText, con, tx))
						{
							AddSqlParametersTo(cmd, item);

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
					string cmdText = $@"DELETE FROM [{TableNames.DEVICE_TABLE}] WHERE [{ID_COLUMN}]=@Id";

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