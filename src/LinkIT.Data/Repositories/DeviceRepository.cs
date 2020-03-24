using LinkIT.Data.Builders;
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

		protected override void AddParametersFor(DeviceDto input, SqlParameterBuilder builder) =>
			builder.ForParameter(input.Id, ID_COLUMN, SqlDbType.BigInt)
				.ForParameter(input.Tag, TAG_COLUMN, SqlDbType.NVarChar)
				.ForParameter(input.Owner, OWNER_COLUMN, SqlDbType.NVarChar)
				.ForParameter(input.Brand, BRAND_COLUMN, SqlDbType.NVarChar)
				.ForParameter(input.Type, TYPE_COLUMN, SqlDbType.NVarChar);

		protected override void AddParametersFor(DeviceQuery input, WhereClauseBuilder builder) =>
			builder.ForParameter(input.Id, ID_COLUMN, SqlDbType.BigInt)
				.ForParameter(input.Tag, TAG_COLUMN, SqlDbType.NVarChar)
				.ForParameter(input.Owner, OWNER_COLUMN, SqlDbType.NVarChar)
				.ForParameter(input.Brand, BRAND_COLUMN, SqlDbType.NVarChar)
				.ForParameter(input.Type, TYPE_COLUMN, SqlDbType.NVarChar);

		protected override IEnumerable<DeviceDto> ReadDtosFrom(SqlDataReader reader)
		{
			while (reader.Read())
			{
				yield return new DeviceDto
				{
					Id = GetColumnValue<long>(reader, ID_COLUMN),
					Tag = GetColumnValue<string>(reader, TAG_COLUMN),
					Owner = GetColumnValue<string>(reader, OWNER_COLUMN),
					Brand = GetColumnValue<string>(reader, BRAND_COLUMN),
					Type = GetColumnValue<string>(reader, TYPE_COLUMN)
				};
			}
		}

		protected override string CreateInsertStatement() =>
			$@"INSERT INTO [{TableName}] 
					([{TAG_COLUMN}], [{OWNER_COLUMN}], [{BRAND_COLUMN}], [{TYPE_COLUMN}]) 
				VALUES (@Tag, @Owner, @Brand, @Type)";

		protected override string CreateUpdateStatement() =>
			$@"UPDATE [{TableName}] SET 
					[{TAG_COLUMN}]=@Tag, [{OWNER_COLUMN}]=@Owner, [{BRAND_COLUMN}]=@Brand, [{TYPE_COLUMN}]=@Type 
				WHERE [{ID_COLUMN}]=@Id";

		public override IEnumerable<string> Columns => 
			COLUMNS;
	}
}