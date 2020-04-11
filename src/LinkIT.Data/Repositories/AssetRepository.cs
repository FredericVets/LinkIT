using LinkIT.Data.Builders;
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
	public class AssetRepository : Repository<AssetDto, AssetQuery>, IAssetRepository
	{
		public const string ICTS_REFERENCE_COLUMN = "IctsReference";
		public const string TAG_COLUMN = "Tag";
		public const string SERIAL_COLUMN = "Serial";
		public const string PRODUCT_ID_COLUMN = "ProductId";
		public const string DESCRIPTION_COLUMN = "Description";
		public const string INVOICE_DATE_COLUMN = "InvoiceDate";
		public const string INVOICE_NUMBER_COLUMN = "InvoiceNumber";
		public const string PRICE_COLUMN = "Price";
		public const string PAID_BY_COLUMN = "PaidBy";
		public const string OWNER_COLUMN = "Owner";
		public const string INSTALL_DATE_COLUMN = "InstallDate";
		public const string INSTALLED_BY_COLUMN = "InstalledBy";
		public const string REMARK_COLUMN = "Remark";
		public const string TEAMASSET_COLUMN = "TeamAsset";

		private static readonly string[] COLUMNS = new[]
		{
			ID_COLUMN, CREATION_DATE_COLUMN, CREATED_BY_COLUMN, MODIFICATION_DATE_COLUMN, MODIFIED_BY_COLUMN, ICTS_REFERENCE_COLUMN,
			TAG_COLUMN, SERIAL_COLUMN, PRODUCT_ID_COLUMN, DESCRIPTION_COLUMN, INVOICE_DATE_COLUMN, INVOICE_NUMBER_COLUMN, PRICE_COLUMN,
			PAID_BY_COLUMN, OWNER_COLUMN, INSTALL_DATE_COLUMN, INSTALLED_BY_COLUMN, REMARK_COLUMN, TEAMASSET_COLUMN
		};

		private readonly IRepository<ProductDto, ProductQuery> _productRepo;

		public AssetRepository(
			ConnectionString connString,
			IRepository<ProductDto, ProductQuery> productRepo) : base(
				connString, TableNames.ASSET_TABLE, hasSoftDelete: true) =>
			_productRepo = productRepo ?? throw new ArgumentNullException(nameof(productRepo));

		private void LinkProductsTo(IEnumerable<AssetDto> assets)
		{
			if (!assets.Any())
				return;

			var productIds = assets.Select(x => x.Product.Id.Value).Distinct();
			var products = _productRepo.GetById(productIds);

			foreach (var asset in assets)
				asset.Product = products.Single(x => x.Id == asset.Product.Id);
		}

		protected override void AddParametersFor(AssetDto input, SqlParameterBuilder builder) =>
			builder.ForParameter(input.Id, ID_COLUMN, SqlDbType.BigInt)
				.ForParameter(input.CreationDate, CREATION_DATE_COLUMN, SqlDbType.DateTime2)
				.ForParameter(input.CreatedBy, CREATED_BY_COLUMN, SqlDbType.VarChar)
				.ForParameter(input.ModificationDate, MODIFICATION_DATE_COLUMN, SqlDbType.DateTime2)
				.ForParameter(input.ModifiedBy, MODIFIED_BY_COLUMN, SqlDbType.VarChar)

				.ForParameter(input.IctsReference, ICTS_REFERENCE_COLUMN, SqlDbType.VarChar)
				.ForParameter(input.Tag, TAG_COLUMN, SqlDbType.VarChar)
				.ForParameter(input.Serial, SERIAL_COLUMN, SqlDbType.VarChar)
				.ForParameter(input.Product.Id, PRODUCT_ID_COLUMN, SqlDbType.BigInt)
				.ForParameter(input.Description, DESCRIPTION_COLUMN, SqlDbType.VarChar)
				.ForParameter(input.InvoiceDate, INVOICE_DATE_COLUMN, SqlDbType.DateTime2)
				.ForParameter(input.InvoiceNumber, INVOICE_NUMBER_COLUMN, SqlDbType.VarChar)
				.ForParameter(input.Price, PRICE_COLUMN, SqlDbType.Decimal)
				.ForParameter(input.PaidBy, PAID_BY_COLUMN, SqlDbType.VarChar)
				.ForParameter(input.Owner, OWNER_COLUMN, SqlDbType.VarChar)
				.ForParameter(input.InstallDate, INSTALL_DATE_COLUMN, SqlDbType.DateTime2)
				.ForParameter(input.InstalledBy, INSTALLED_BY_COLUMN, SqlDbType.VarChar)
				.ForParameter(input.Remark, REMARK_COLUMN, SqlDbType.VarChar)
				.ForParameter(input.TeamAsset, TEAMASSET_COLUMN, SqlDbType.Bit);

		protected override void AddParametersFor(AssetQuery input, WhereClauseBuilder builder) =>
			builder.ForParameter(input.Id, ID_COLUMN, SqlDbType.BigInt)
				.ForParameter(input.CreationDate, CREATION_DATE_COLUMN, SqlDbType.DateTime2)
				.ForParameter(input.CreatedBy, CREATED_BY_COLUMN, SqlDbType.VarChar)
				.ForParameter(input.ModificationDate, MODIFICATION_DATE_COLUMN, SqlDbType.DateTime2)
				.ForParameter(input.ModifiedBy, MODIFIED_BY_COLUMN, SqlDbType.VarChar)

				.ForParameter(input.IctsReference, ICTS_REFERENCE_COLUMN, SqlDbType.VarChar)
				.ForParameter(input.Tag, TAG_COLUMN, SqlDbType.VarChar)
				.ForParameter(input.Serial, SERIAL_COLUMN, SqlDbType.VarChar)
				.ForParameter(input.ProductId, PRODUCT_ID_COLUMN, SqlDbType.BigInt)
				.ForParameter(input.Description, DESCRIPTION_COLUMN, SqlDbType.VarChar)
				.ForParameter(input.InvoiceDate, INVOICE_DATE_COLUMN, SqlDbType.DateTime2)
				.ForParameter(input.InvoiceNumber, INVOICE_NUMBER_COLUMN, SqlDbType.VarChar)
				.ForParameter(input.Price, PRICE_COLUMN, SqlDbType.Decimal)
				.ForParameter(input.PaidBy, PAID_BY_COLUMN, SqlDbType.VarChar)
				.ForParameter(input.Owner, OWNER_COLUMN, SqlDbType.VarChar)
				.ForParameter(input.InstallDate, INSTALL_DATE_COLUMN, SqlDbType.DateTime2)
				.ForParameter(input.InstalledBy, INSTALLED_BY_COLUMN, SqlDbType.VarChar)
				.ForParameter(input.Remark, REMARK_COLUMN, SqlDbType.VarChar)
				.ForParameter(input.TeamAsset, TEAMASSET_COLUMN, SqlDbType.Bit);

		protected override IEnumerable<AssetDto> ReadDtosFrom(SqlDataReader reader)
		{
			while (reader.Read())
			{
				var dto = new AssetDto();
				Populate(reader, dto);

				yield return dto;
			}
		}

		protected override string CreateInsertStatement() =>
			$@"INSERT INTO [{TableName}] 
				([{CREATION_DATE_COLUMN}], [{CREATED_BY_COLUMN}], [{MODIFICATION_DATE_COLUMN}], [{MODIFIED_BY_COLUMN}], 
				[{ICTS_REFERENCE_COLUMN}], [{TAG_COLUMN}], [{SERIAL_COLUMN}], [{PRODUCT_ID_COLUMN}], [{DESCRIPTION_COLUMN}],
				[{INVOICE_DATE_COLUMN}], [{INVOICE_NUMBER_COLUMN}], [{PRICE_COLUMN}], [{PAID_BY_COLUMN}], [{OWNER_COLUMN}], 
				[{INSTALL_DATE_COLUMN}], [{INSTALLED_BY_COLUMN}], [{REMARK_COLUMN}], [{TEAMASSET_COLUMN}], [{DELETED_COLUMN}])
			VALUES (@{CREATION_DATE_COLUMN}, @{CREATED_BY_COLUMN}, @{MODIFICATION_DATE_COLUMN}, @{MODIFIED_BY_COLUMN}, 
				@{ICTS_REFERENCE_COLUMN}, @{TAG_COLUMN}, @{SERIAL_COLUMN}, @{PRODUCT_ID_COLUMN}, @{DESCRIPTION_COLUMN}, 
				@{INVOICE_DATE_COLUMN}, @{INVOICE_NUMBER_COLUMN}, @{PRICE_COLUMN}, @{PAID_BY_COLUMN}, @{OWNER_COLUMN}, 
				@{INSTALL_DATE_COLUMN}, @{INSTALLED_BY_COLUMN}, @{REMARK_COLUMN}, @{TEAMASSET_COLUMN}, 0)";

		protected override string CreateUpdateStatement() =>
			$@"UPDATE [{TableName}] SET
				[{MODIFICATION_DATE_COLUMN}]=@{MODIFICATION_DATE_COLUMN}, [{MODIFIED_BY_COLUMN}]=@{MODIFIED_BY_COLUMN}, 
				[{ICTS_REFERENCE_COLUMN}]=@{ICTS_REFERENCE_COLUMN}, [{TAG_COLUMN}]=@{TAG_COLUMN}, [{SERIAL_COLUMN}]=@{SERIAL_COLUMN}, 
				[{PRODUCT_ID_COLUMN}]=@{PRODUCT_ID_COLUMN}, [{DESCRIPTION_COLUMN}]=@{DESCRIPTION_COLUMN}, 
				[{INVOICE_DATE_COLUMN}]=@{INVOICE_DATE_COLUMN}, [{INVOICE_NUMBER_COLUMN}]=@{INVOICE_NUMBER_COLUMN}, 
				[{PRICE_COLUMN}]=@{PRICE_COLUMN}, [{PAID_BY_COLUMN}]=@{PAID_BY_COLUMN}, [{OWNER_COLUMN}]=@{OWNER_COLUMN}, 
				[{INSTALL_DATE_COLUMN}]=@{INSTALL_DATE_COLUMN}, [{INSTALLED_BY_COLUMN}]=@{INSTALLED_BY_COLUMN}, 
				[{REMARK_COLUMN}]=@{REMARK_COLUMN}, [{TEAMASSET_COLUMN}]=@{TEAMASSET_COLUMN}
			WHERE [{ID_COLUMN}]=@{ID_COLUMN} AND [{DELETED_COLUMN}]=0";

		public static void Populate(SqlDataReader from, AssetDto to)
		{
			from = from ?? throw new ArgumentNullException(nameof(from));
			to = to ?? throw new ArgumentNullException(nameof(to));

			to.Id = GetColumnValue<long>(from, ID_COLUMN);
			to.CreationDate = GetColumnValue<DateTime>(from, CREATION_DATE_COLUMN);
			to.CreatedBy = GetColumnValue<string>(from, CREATED_BY_COLUMN);
			to.ModificationDate = GetColumnValue<DateTime>(from, MODIFICATION_DATE_COLUMN);
			to.ModifiedBy = GetColumnValue<string>(from, MODIFIED_BY_COLUMN);

			to.IctsReference = GetColumnValue<string>(from, ICTS_REFERENCE_COLUMN);
			to.Tag = GetColumnValue<string>(from, TAG_COLUMN);
			to.Serial = GetColumnValue<string>(from, SERIAL_COLUMN);
			to.Product = new ProductDto
			{
				Id = GetColumnValue<long?>(from, PRODUCT_ID_COLUMN)
			};
			to.Description = GetColumnValue<string>(from, DESCRIPTION_COLUMN);
			to.InvoiceDate = GetColumnValue<DateTime?>(from, INVOICE_DATE_COLUMN);
			to.InvoiceNumber = GetColumnValue<string>(from, INVOICE_NUMBER_COLUMN);
			to.Price = GetColumnValue<decimal?>(from, PRICE_COLUMN);
			to.PaidBy = GetColumnValue<string>(from, PAID_BY_COLUMN);
			to.Owner = GetColumnValue<string>(from, OWNER_COLUMN);
			to.InstallDate = GetColumnValue<DateTime?>(from, INSTALL_DATE_COLUMN);
			to.InstalledBy = GetColumnValue<string>(from, INSTALLED_BY_COLUMN);
			to.Remark = GetColumnValue<string>(from, REMARK_COLUMN);
			to.TeamAsset = GetColumnValue<bool>(from, TEAMASSET_COLUMN);
		}

		public override IEnumerable<string> Columns => 
			COLUMNS;

		public override IEnumerable<AssetDto> GetById(IEnumerable<long> ids)
		{
			var assets = base.GetById(ids);
			LinkProductsTo(assets);

			return assets;
		}

		public override IEnumerable<AssetDto> Query(AssetQuery query = null)
		{
			var assets = base.Query(query);
			LinkProductsTo(assets);

			return assets;
		}

		public override PagedResult<AssetDto> PagedQuery(PageInfo pageInfo, AssetQuery query = null)
		{
			var pagedResult = base.PagedQuery(pageInfo, query);
			var assets = pagedResult.Result;
			LinkProductsTo(assets);

			return new PagedResult<AssetDto>(assets, pagedResult.PageInfo, pagedResult.TotalCount);
		}

		public override long Insert(AssetDto item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));
			if (item.Id.HasValue)
				throw new ArgumentException("Id can not be specified.");
			if (string.IsNullOrWhiteSpace(item.CreatedBy))
				throw new ArgumentException("CreatedBy is required!");
			if (item.Product == null || !item.Product.Id.HasValue)
				throw new ArgumentException("Product Id is required!");

			item.ModifiedBy = item.CreatedBy;
			item.ModificationDate = item.CreationDate = DateTimeProvider.Now();

			return base.Insert(item);
		}

		public override void Update(IEnumerable<AssetDto> items)
		{
			if (items == null || !items.Any())
				throw new ArgumentNullException(nameof(items));

			var now = DateTimeProvider.Now();
			foreach (var item in items)
			{
				if (!item.Id.HasValue)
					throw new ArgumentException("Id is a required field.");
				if (string.IsNullOrWhiteSpace(item.ModifiedBy))
					throw new ArgumentException("ModifiedBy is required!");
				if (item.Product == null || !item.Product.Id.HasValue)
					throw new ArgumentException("Product Id is required!");

				item.ModificationDate = now;
			}

			base.Update(items);
		}

		public IEnumerable<AssetDto> ForOwners(IEnumerable<string> owners)
		{
			if (owners == null || !owners.Any())
				throw new ArgumentNullException(nameof(owners));

			IList<AssetDto> assets;
			using (var con = new SqlConnection(ConnectionString.Value))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = new SelectCommandBuilder(con, tx, HasSoftDelete)
						.ForSelect(CreateSelectStatement())
						.ForWhereIn(OWNER_COLUMN, owners, SqlDbType.VarChar)
						.Build())
					using (var reader = cmd.ExecuteReader())
					{
						assets = ReadDtosFrom(reader).ToList();
					}

					//tx.Commit();
				}
			}

			LinkProductsTo(assets);

			return assets;
		}
	}
}