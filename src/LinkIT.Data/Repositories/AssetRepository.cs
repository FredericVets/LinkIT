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
	public class AssetRepository : Repository<AssetDto, AssetQuery>
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
			string connectionString, 
			IRepository<ProductDto, ProductQuery> productRepo) : base(
				connectionString, TableNames.ASSET_TABLE, hasSoftDelete: true)
		{
			_productRepo = productRepo;
		}

		private void LinkProductsTo(IEnumerable<AssetDto> assets)
		{
			var productIds = assets.Select(x => x.Product.Id.Value).Distinct();
			var products = _productRepo.GetById(productIds);

			foreach (var asset in assets)
				asset.Product = products.Single(x => x.Id == asset.Product.Id);
		}

		protected override void BuildParametersFrom(AssetDto input, SqlParameterBuilder builder)
		{
			builder.AddParameter(input.Id, ID_COLUMN, SqlDbType.BigInt);
			builder.AddParameter(input.CreationDate, CREATION_DATE_COLUMN, SqlDbType.DateTime2);
			builder.AddParameter(input.CreatedBy, CREATED_BY_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.ModificationDate, MODIFICATION_DATE_COLUMN, SqlDbType.DateTime2);
			builder.AddParameter(input.ModifiedBy, MODIFIED_BY_COLUMN, SqlDbType.VarChar);

			builder.AddParameter(input.IctsReference, ICTS_REFERENCE_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.Tag, TAG_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.Serial, SERIAL_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.Product.Id, PRODUCT_ID_COLUMN, SqlDbType.BigInt);
			builder.AddParameter(input.Description, DESCRIPTION_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.InvoiceDate, INVOICE_DATE_COLUMN, SqlDbType.DateTime2);
			builder.AddParameter(input.InvoiceNumber, INVOICE_NUMBER_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.Price, PRICE_COLUMN, SqlDbType.Decimal);
			builder.AddParameter(input.PaidBy, PAID_BY_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.Owner, OWNER_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.InstallDate, INSTALL_DATE_COLUMN, SqlDbType.DateTime2);
			builder.AddParameter(input.InstalledBy, INSTALLED_BY_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.Remark, REMARK_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.TeamAsset, TEAMASSET_COLUMN, SqlDbType.Bit);
		}

		protected override void BuildParametersFrom(AssetQuery input, WhereClauseBuilder builder)
		{
			builder.AddParameter(input.Id, ID_COLUMN, SqlDbType.BigInt);
			builder.AddParameter(input.CreationDate, CREATION_DATE_COLUMN, SqlDbType.DateTime2);
			builder.AddParameter(input.CreatedBy, CREATED_BY_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.ModificationDate, MODIFICATION_DATE_COLUMN, SqlDbType.DateTime2);
			builder.AddParameter(input.ModifiedBy, MODIFIED_BY_COLUMN, SqlDbType.VarChar);

			builder.AddParameter(input.IctsReference, ICTS_REFERENCE_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.Tag, TAG_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.Serial, SERIAL_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.ProductId, PRODUCT_ID_COLUMN, SqlDbType.BigInt);
			builder.AddParameter(input.Description, DESCRIPTION_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.InvoiceDate, INVOICE_DATE_COLUMN, SqlDbType.DateTime2);
			builder.AddParameter(input.InvoiceNumber, INVOICE_NUMBER_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.Price, PRICE_COLUMN, SqlDbType.Decimal);
			builder.AddParameter(input.PaidBy, PAID_BY_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.Owner, OWNER_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.InstallDate, INSTALL_DATE_COLUMN, SqlDbType.DateTime2);
			builder.AddParameter(input.InstalledBy, INSTALLED_BY_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.Remark, REMARK_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.TeamAsset, TEAMASSET_COLUMN, SqlDbType.Bit);
		}

		protected override IEnumerable<AssetDto> ReadDtosFrom(SqlDataReader reader)
		{
			while (reader.Read())
			{
				yield return new AssetDto
				{
					Id = GetColumnValue<long?>(reader, ID_COLUMN),
					CreationDate = GetColumnValue<DateTime?>(reader, CREATION_DATE_COLUMN),
					CreatedBy = GetColumnValue<string>(reader, CREATED_BY_COLUMN),
					ModificationDate = GetColumnValue<DateTime?>(reader, MODIFICATION_DATE_COLUMN),
					ModifiedBy = GetColumnValue<string>(reader, MODIFIED_BY_COLUMN),
					IctsReference = GetColumnValue<string>(reader, ICTS_REFERENCE_COLUMN),
					Tag = GetColumnValue<string>(reader, TAG_COLUMN),
					Serial = GetColumnValue<string>(reader, SERIAL_COLUMN),
					Product = new ProductDto
					{
						Id = GetColumnValue<long?>(reader, PRODUCT_ID_COLUMN)
					},
					Description = GetColumnValue<string>(reader, DESCRIPTION_COLUMN),
					InvoiceDate = GetColumnValue<DateTime?>(reader, INVOICE_DATE_COLUMN),
					InvoiceNumber = GetColumnValue<string>(reader, INVOICE_NUMBER_COLUMN),
					Price = GetColumnValue<decimal?>(reader, PRICE_COLUMN),
					PaidBy = GetColumnValue<string>(reader, PAID_BY_COLUMN),
					Owner = GetColumnValue<string>(reader, OWNER_COLUMN),
					InstallDate = GetColumnValue<DateTime?>(reader, INSTALL_DATE_COLUMN),
					InstalledBy = GetColumnValue<string>(reader, INSTALLED_BY_COLUMN),
					Remark = GetColumnValue<string>(reader, REMARK_COLUMN),
					TeamAsset = GetColumnValue<bool?>(reader, TEAMASSET_COLUMN)
				};
			}
		}

		protected override string CreateInsertStatement()
		{
			return
				$@"INSERT INTO [{TableName}] 
					([{CREATION_DATE_COLUMN}], [{CREATED_BY_COLUMN}], [{MODIFICATION_DATE_COLUMN}], [{MODIFIED_BY_COLUMN}], 
					[{ICTS_REFERENCE_COLUMN}], [{TAG_COLUMN}], [{SERIAL_COLUMN}], [{PRODUCT_ID_COLUMN}], [{DESCRIPTION_COLUMN}],
					[{INVOICE_DATE_COLUMN}], [{INVOICE_NUMBER_COLUMN}], [{PRICE_COLUMN}], [{PAID_BY_COLUMN}], [{OWNER_COLUMN}], 
					[{INSTALL_DATE_COLUMN}], [{INSTALLED_BY_COLUMN}], [{REMARK_COLUMN}], [{TEAMASSET_COLUMN}], [{DELETED_COLUMN}])
				VALUES (@{CREATION_DATE_COLUMN}, @{CREATED_BY_COLUMN}, @{MODIFICATION_DATE_COLUMN}, @{MODIFIED_BY_COLUMN}, 
					@{ICTS_REFERENCE_COLUMN}, @{TAG_COLUMN}, @{SERIAL_COLUMN}, @{PRODUCT_ID_COLUMN}, @{DESCRIPTION_COLUMN}, 
					@{INVOICE_DATE_COLUMN}, @{INVOICE_NUMBER_COLUMN}, @{PRICE_COLUMN}, @{PAID_BY_COLUMN}, @{OWNER_COLUMN}, 
					@{INSTALL_DATE_COLUMN}, @{INSTALLED_BY_COLUMN}, @{REMARK_COLUMN}, @{TEAMASSET_COLUMN}, 0)";
		}

		protected override string CreateUpdateStatement()
		{
			return
				$@"UPDATE [{TableName}] SET
					[{MODIFICATION_DATE_COLUMN}]=@{MODIFICATION_DATE_COLUMN}, [{MODIFIED_BY_COLUMN}]=@{MODIFIED_BY_COLUMN}, 
					[{ICTS_REFERENCE_COLUMN}]=@{ICTS_REFERENCE_COLUMN}, [{TAG_COLUMN}]=@{TAG_COLUMN}, [{SERIAL_COLUMN}]=@{SERIAL_COLUMN}, 
					[{PRODUCT_ID_COLUMN}]=@{PRODUCT_ID_COLUMN}, [{DESCRIPTION_COLUMN}]=@{DESCRIPTION_COLUMN}, 
					[{INVOICE_DATE_COLUMN}]=@{INVOICE_DATE_COLUMN}, [{INVOICE_NUMBER_COLUMN}]=@{INVOICE_NUMBER_COLUMN}, 
					[{PRICE_COLUMN}]=@{PRICE_COLUMN}, [{PAID_BY_COLUMN}]=@{PAID_BY_COLUMN}, [{OWNER_COLUMN}]=@{OWNER_COLUMN}, 
					[{INSTALL_DATE_COLUMN}]=@{INSTALL_DATE_COLUMN}, [{INSTALLED_BY_COLUMN}]=@{INSTALLED_BY_COLUMN}, 
					[{REMARK_COLUMN}]=@{REMARK_COLUMN}, [{TEAMASSET_COLUMN}]=@{TEAMASSET_COLUMN}
				WHERE [{ID_COLUMN}]=@{ID_COLUMN} AND [{DELETED_COLUMN}]=0";
		}

		public override IEnumerable<string> Columns => COLUMNS;

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
				throw new ArgumentNullException("item");
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
				throw new ArgumentNullException("items");

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
	}
}