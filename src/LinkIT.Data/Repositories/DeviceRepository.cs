using LinkIT.Data.DTO;
using LinkIT.Data.Queries;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace LinkIT.Data.Repositories
{
	public class DeviceRepository : Repository<DeviceDto, DeviceQuery>
	{
		public const string TAG_COLUMN = "Tag";
		public const string OWNER_COLUMN = "Owner";
		public const string BRAND_COLUMN = "Brand";
		public const string TYPE_COLUMN = "Type";

		public static readonly string[] COLUMNS = new[] { ID_COLUMN, TAG_COLUMN, OWNER_COLUMN, BRAND_COLUMN, TYPE_COLUMN };

		public DeviceRepository(string connectionString) : base(connectionString, TableNames.DEVICE_TABLE) { }

		protected override IEnumerable<string> Columns => COLUMNS;

		protected override IEnumerable<DeviceDto> ReadDtosFrom(SqlDataReader reader)
		{
			while (reader.Read())
			{
				yield return new DeviceDto
				{
					Id = GetColumnValue<long?>(reader, ID_COLUMN),
					Tag = GetColumnValue<string>(reader, TAG_COLUMN),
					Owner = GetColumnValue<string>(reader, OWNER_COLUMN),
					Brand = GetColumnValue<string>(reader, BRAND_COLUMN),
					Type = GetColumnValue<string>(reader, TYPE_COLUMN)
				};
			}
		}

		protected override void AddWhereClause(SqlParameterCollection @params, StringBuilder sb, DeviceQuery query)
		{
			var where = new WhereClauseBuilder(@params, query.LogicalOperator, false);
			where.AddParameter(query.Id, ID_COLUMN, SqlDbType.BigInt);
			where.AddParameter(query.Tag, TAG_COLUMN, SqlDbType.NVarChar);
			where.AddParameter(query.Owner, OWNER_COLUMN, SqlDbType.NVarChar);
			where.AddParameter(query.Brand, BRAND_COLUMN, SqlDbType.NVarChar);
			where.AddParameter(query.Type, TYPE_COLUMN, SqlDbType.NVarChar);

			sb.Append(where.Build());
		}

		protected override string CreateInsertStatement()
		{
			return $@"INSERT INTO [{TableName}] 
						([{TAG_COLUMN}], [{OWNER_COLUMN}], [{BRAND_COLUMN}], [{TYPE_COLUMN}]) 
					VALUES (@Tag, @Owner, @Brand, @Type)";
		}

		protected override string CreateUpdateStatement()
		{
			return $@"UPDATE [{TableName}] SET 
						[{TAG_COLUMN}]=@Tag, [{OWNER_COLUMN}]=@Owner, [{BRAND_COLUMN}]=@Brand, [{TYPE_COLUMN}]=@Type 
					WHERE [{ID_COLUMN}]=@Id";
		}

		protected override void AddSqlParameters(SqlParameterCollection @params, DeviceDto input)
		{
			var paramBuilder = new SqlParameterBuilder(@params);
			paramBuilder.Add(input.Id, ID_COLUMN, SqlDbType.BigInt);
			paramBuilder.Add(input.Tag, TAG_COLUMN, SqlDbType.NVarChar);
			paramBuilder.Add(input.Owner, OWNER_COLUMN, SqlDbType.NVarChar);
			paramBuilder.Add(input.Brand, BRAND_COLUMN, SqlDbType.NVarChar);
			paramBuilder.Add(input.Type, TYPE_COLUMN, SqlDbType.NVarChar);
		}
	}
}