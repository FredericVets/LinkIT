using LinkIT.Data.DTO;
using LinkIT.Data.Queries;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace LinkIT.Data.Repositories
{
	public class DeviceRepository : Repository<DeviceDto, DeviceQuery>
	{
		public const string TAG_COLUMN = "Tag";
		public const string OWNER_COLUMN = "Owner";
		public const string BRAND_COLUMN = "Brand";
		public const string TYPE_COLUMN = "Type";

		private static readonly string[] COLUMNS = new[] { ID_COLUMN, TAG_COLUMN, OWNER_COLUMN, BRAND_COLUMN, TYPE_COLUMN };

		public DeviceRepository(string connectionString) : base(connectionString, TableNames.DEVICE_TABLE) { }

		protected override SqlParameterBuilder BuildParametersFrom(DeviceDto input, SqlParameterCollection @params)
		{
			var builder = new SqlParameterBuilder(@params);

			builder.Add(input.Id, ID_COLUMN, SqlDbType.BigInt);
			builder.Add(input.Tag, TAG_COLUMN, SqlDbType.NVarChar);
			builder.Add(input.Owner, OWNER_COLUMN, SqlDbType.NVarChar);
			builder.Add(input.Brand, BRAND_COLUMN, SqlDbType.NVarChar);
			builder.Add(input.Type, TYPE_COLUMN, SqlDbType.NVarChar);

			return builder;
		}

		protected override WhereClauseBuilder BuildParametersFrom(DeviceQuery input, SqlParameterCollection @params)
		{
			var builder = new WhereClauseBuilder(@params, input.LogicalOperator, false);

			builder.AddParameter(input.Id, ID_COLUMN, SqlDbType.BigInt);
			builder.AddParameter(input.Tag, TAG_COLUMN, SqlDbType.NVarChar);
			builder.AddParameter(input.Owner, OWNER_COLUMN, SqlDbType.NVarChar);
			builder.AddParameter(input.Brand, BRAND_COLUMN, SqlDbType.NVarChar);
			builder.AddParameter(input.Type, TYPE_COLUMN, SqlDbType.NVarChar);

			return builder;
		}

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

		public override IEnumerable<string> Columns => COLUMNS;
	}
}