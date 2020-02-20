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

		public IEnumerable<DeviceDto> GetAll()
		{
			using (var con = new SqlConnection(_connectionString))
			using (var tx = con.BeginTransaction())
			{
				using (var cmd = CreateSelectCommand(con, tx))
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						yield return new DeviceDto
						{
							Id = (Guid)reader[ID_COLUMN],
							Tag = reader[TAG_COLUMN].ToString(),
							Owner = reader[OWNER_COLUMN].ToString(),
							Brand = reader[BRAND_COLUMN].ToString(),
							Type = reader[TYPE_COLUMN].ToString()
						};
					}
				}

				//tx.Commit();
			}
		}

		public DeviceDto GetById(Guid deviceId)
		{
			return Query(new DeviceQuery { Id = deviceId }).Single();
		}

		public IEnumerable<DeviceDto> Query(DeviceQuery query)
		{
			using (var con = new SqlConnection(_connectionString))
			using (var tx = con.BeginTransaction())
			{
				using (var cmd = CreateSelectCommandWithConditions(con, tx, query))
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						yield return new DeviceDto
						{
							Id = (Guid)reader[ID_COLUMN],
							Tag = reader[TAG_COLUMN].ToString(),
							Owner = reader[OWNER_COLUMN].ToString(),
							Brand = reader[BRAND_COLUMN].ToString(),
							Type = reader[TYPE_COLUMN].ToString()
						};
					}
				}

				//tx.Commit();
			}
		}
	}
}