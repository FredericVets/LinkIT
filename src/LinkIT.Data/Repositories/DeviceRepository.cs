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
	public class DeviceRepository
	{
		private const string ID_COLUMN = "Id";
		private const string TAG_COLUMN = "Tag";
		private const string OWNER_COLUMN = "Owner";
		private const string BRAND_COLUMN = "Brand";
		private const string TYPE_COLUMN = "Type";

		private string _connectionString;

		public DeviceRepository(string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString))
				throw new ArgumentNullException();

			_connectionString = connectionString;
		}

		private static SqlCommand CreateSelectCommand(SqlConnection con, SqlTransaction tx)
		{
			string cmdText = string.Format("SELECT * FROM {0}", TableNames.DEVICE_TABLE);

			return new SqlCommand(cmdText, con, tx);
		}

		// To add parameters to a command, see : https://stackoverflow.com/questions/293311/whats-the-best-method-to-pass-parameters-to-sqlcommand
		private static SqlCommand CreateSelectCommandWithConditions(
			SqlConnection con,
			SqlTransaction tx,
			DeviceQuery query,
			SelectCondition condition = SelectCondition.AND)
		{
			var sb = new StringBuilder();
			sb.AppendFormat("SELECT * FROM {0} WHERE", TableNames.DEVICE_TABLE);
			sb.AppendLine();

			var cmd = new SqlCommand { Connection = con, Transaction = tx };

			bool firstCondition = true;
			if (query.Id.HasValue)
			{
				sb.AppendFormat("{0} = @Id", ID_COLUMN);
				sb.AppendLine();
				cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = query.Id.Value;
				firstCondition = false;
			}

			if (!string.IsNullOrWhiteSpace(query.Tag))
			{
				if (!firstCondition)
					sb.AppendLine(condition.ToString());

				sb.AppendFormat("{0} = @Tag", TAG_COLUMN);
				sb.AppendLine();
				cmd.Parameters.Add("@Tag", SqlDbType.NVarChar).Value = query.Tag;
				firstCondition = false;
			}

			if (!string.IsNullOrWhiteSpace(query.Owner))
			{
				if (!firstCondition)
					sb.AppendLine(condition.ToString());

				sb.AppendFormat("{0} = @Owner", OWNER_COLUMN);
				sb.AppendLine();
				cmd.Parameters.Add("@Owner", SqlDbType.NVarChar).Value = query.Owner;
				firstCondition = false;
			}

			if (!string.IsNullOrWhiteSpace(query.Brand))
			{
				if (!firstCondition)
					sb.AppendLine(condition.ToString());

				sb.AppendFormat("{0} = @Brand", BRAND_COLUMN);
				sb.AppendLine();
				cmd.Parameters.Add("@Brand", SqlDbType.NVarChar).Value = query.Brand;
				firstCondition = false;
			}

			if (!string.IsNullOrWhiteSpace(query.Type))
			{
				if (!firstCondition)
					sb.AppendLine(condition.ToString());

				sb.AppendFormat("{0} = @Type", TYPE_COLUMN);
				sb.AppendLine();
				cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = query.Type;
				firstCondition = false;
			}

			cmd.CommandText = sb.ToString();

			return cmd;
		}

		public DeviceDto GetById(Guid id)
		{
			var result = Query(new DeviceQuery { Id = id }).ToList();

			if (result.Count == 0)
				throw new InvalidOperationException(string.Format("No device found for id : '{0}'.", id));

			return result.Single();
		}

		public IEnumerable<DeviceDto> GetAll()
		{
			var result = new List<DeviceDto>();

			using (var con = new SqlConnection(_connectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = CreateSelectCommand(con, tx))
					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							result.Add(new DeviceDto
							{
								Id = (Guid)reader[ID_COLUMN],
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

		public IEnumerable<DeviceDto> Query(DeviceQuery query)
		{
			var result = new List<DeviceDto>();

			using (var con = new SqlConnection(_connectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = CreateSelectCommandWithConditions(con, tx, query, SelectCondition.AND))
					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							result.Add(new DeviceDto
							{
								Id = (Guid)reader[ID_COLUMN],
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

		public Guid Insert(DeviceDto input)
		{
			input.ValidateRequiredFields();

			// TODO : id generation server-side or on database by using Identity?
			input.Id = Guid.NewGuid();

			using (var con = new SqlConnection(_connectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText = string.Format(
						@"INSERT into {0} ([{1}], [{2}], [{3}], [{4}], [{5}]) VALUES (@Id, @Tag, @Owner, @Brand, @Type)", 
						TableNames.DEVICE_TABLE,
						ID_COLUMN,
						TAG_COLUMN,
						OWNER_COLUMN,
						BRAND_COLUMN,
						TYPE_COLUMN);

					using (var cmd = new SqlCommand(cmdText, con, tx))
					{
						cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = input.Id.Value;
						cmd.Parameters.Add("@Tag", SqlDbType.NVarChar).Value = input.Tag;
						cmd.Parameters.Add("@Owner", SqlDbType.NVarChar).Value = input.Owner;
						cmd.Parameters.Add("@Brand", SqlDbType.NVarChar).Value = input.Brand;
						cmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = input.Type;

						cmd.ExecuteNonQuery();
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
						@"UPDATE {0} SET {1}=@Tag, {2}=@Owner, {3}=@Brand, {4}=@Type WHERE {5}=@Id", 
						TableNames.DEVICE_TABLE,
						TAG_COLUMN,
						OWNER_COLUMN,
						BRAND_COLUMN,
						TYPE_COLUMN,
						ID_COLUMN);

					using (var cmd = new SqlCommand(cmdText, con, tx))
					{
						cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = input.Id.Value;
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

		public void Delete(Guid id)
		{
			using (var con = new SqlConnection(_connectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText = string.Format(
						@"DELETE FROM {0} WHERE {1}=@Id",
						TableNames.DEVICE_TABLE,
						ID_COLUMN);

					using (var cmd = new SqlCommand(cmdText, con, tx))
					{
						cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

						cmd.ExecuteNonQuery();
					}

					tx.Commit();
				}
			}
		}
	}
}