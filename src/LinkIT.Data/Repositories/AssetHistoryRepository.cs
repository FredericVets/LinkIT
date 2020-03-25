using LinkIT.Data.Builders;
using LinkIT.Data.DTO;
using LinkIT.Data.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace LinkIT.Data.Repositories
{
	public class AssetHistoryRepository : Repository<AssetHistoryDto, AssetHistoryQuery>
	{
		public const string ASSET_ID_COLUMN = "AssetId";

		private static readonly string[] COLUMNS = new[]
		{
			ID_COLUMN, ASSET_ID_COLUMN, CREATION_DATE_COLUMN, CREATED_BY_COLUMN, MODIFICATION_DATE_COLUMN, MODIFIED_BY_COLUMN,
			AssetRepository.ICTS_REFERENCE_COLUMN, AssetRepository.TAG_COLUMN, AssetRepository.SERIAL_COLUMN,
			AssetRepository.PRODUCT_ID_COLUMN, AssetRepository.DESCRIPTION_COLUMN, AssetRepository.INVOICE_DATE_COLUMN,
			AssetRepository.INVOICE_NUMBER_COLUMN, AssetRepository.PRICE_COLUMN, AssetRepository.PAID_BY_COLUMN,
			AssetRepository.OWNER_COLUMN, AssetRepository.INSTALL_DATE_COLUMN, AssetRepository.INSTALLED_BY_COLUMN,
			AssetRepository.REMARK_COLUMN, AssetRepository.TEAMASSET_COLUMN
		};

		public AssetHistoryRepository(string connectionString) : base(connectionString, TableNames.ASSET_HISTORY_TABLE) { }

		protected override void AddParametersFor(AssetHistoryDto input, SqlParameterBuilder builder) =>
			throw new NotImplementedException();

		protected override void AddParametersFor(AssetHistoryQuery input, WhereClauseBuilder builder) =>
			builder.ForParameter(input.Id, ID_COLUMN, SqlDbType.BigInt)
				.ForParameter(input.AssetId, ASSET_ID_COLUMN, SqlDbType.BigInt)
				.ForParameter(input.Tag, AssetRepository.TAG_COLUMN, SqlDbType.VarChar);

		protected override string CreateInsertStatement() =>
			throw new NotImplementedException();

		protected override string CreateUpdateStatement() =>
			throw new NotImplementedException();

		protected override IEnumerable<AssetHistoryDto> ReadDtosFrom(SqlDataReader reader)
		{
			while (reader.Read())
			{
				var dto = new AssetHistoryDto { AssetId = GetColumnValue<long>(reader, ASSET_ID_COLUMN) };
				AssetRepository.Populate(reader, dto);

				yield return dto;
			}
		}

		public override IEnumerable<string> Columns => 
			COLUMNS;

		public override long Insert(AssetHistoryDto item) =>
			throw new InvalidOperationException("An AssetHistory can not be inserted.");

		public override void Update(IEnumerable<AssetHistoryDto> items) =>
			throw new InvalidOperationException("An AssetHistory can not be updated.");

		public override void Delete(long id) =>
			throw new InvalidOperationException("An AssetHistory can not be deleted.");
	}
}