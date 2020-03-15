using LinkIT.Data.DTO;
using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace LinkIT.Data.Repositories
{
	public class AssetRepository : Repository, IRepository<AssetDto, AssetQuery>
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

		public static readonly string[] COLUMNS = new[]
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

		private static IEnumerable<AssetDto> ReadDtosFrom(SqlDataReader reader)
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

		private static void AddSqlParameters(SqlParameterCollection @params, AssetDto input)
		{
			var paramBuilder = new SqlParameterBuilder(@params);
			paramBuilder.Add(input.Id, ID_COLUMN, SqlDbType.BigInt);
			paramBuilder.Add(input.CreationDate, CREATION_DATE_COLUMN, SqlDbType.DateTime2);
			paramBuilder.Add(input.CreatedBy, CREATED_BY_COLUMN, SqlDbType.VarChar);
			paramBuilder.Add(input.ModificationDate, MODIFICATION_DATE_COLUMN, SqlDbType.DateTime2);
			paramBuilder.Add(input.ModifiedBy, MODIFIED_BY_COLUMN, SqlDbType.VarChar);

			paramBuilder.Add(input.IctsReference, ICTS_REFERENCE_COLUMN, SqlDbType.VarChar);
			paramBuilder.Add(input.Tag, TAG_COLUMN, SqlDbType.VarChar);
			paramBuilder.Add(input.Serial, SERIAL_COLUMN, SqlDbType.VarChar);
			paramBuilder.Add(input.Product.Id, PRODUCT_ID_COLUMN, SqlDbType.BigInt);
			paramBuilder.Add(input.Description, DESCRIPTION_COLUMN, SqlDbType.VarChar);
			paramBuilder.Add(input.InvoiceDate, INVOICE_DATE_COLUMN, SqlDbType.DateTime2);
			paramBuilder.Add(input.InvoiceNumber, INVOICE_NUMBER_COLUMN, SqlDbType.VarChar);
			paramBuilder.Add(input.Price, PRICE_COLUMN, SqlDbType.Decimal);
			paramBuilder.Add(input.PaidBy, PAID_BY_COLUMN, SqlDbType.VarChar);
			paramBuilder.Add(input.Owner, OWNER_COLUMN, SqlDbType.VarChar);
			paramBuilder.Add(input.InstallDate, INSTALL_DATE_COLUMN, SqlDbType.DateTime2);
			paramBuilder.Add(input.InstalledBy, INSTALLED_BY_COLUMN, SqlDbType.VarChar);
			paramBuilder.Add(input.Remark, REMARK_COLUMN, SqlDbType.VarChar);
			paramBuilder.Add(input.TeamAsset, TEAMASSET_COLUMN, SqlDbType.Bit);
		}

		private static void AddWhereClause(SqlParameterCollection @params, StringBuilder sb, AssetQuery query)
		{
			var where = new WhereClauseBuilder(@params, query.LogicalOperator, true);
			where.AddParameter(query.Id, ID_COLUMN, SqlDbType.BigInt);
			where.AddParameter(query.CreationDate, CREATION_DATE_COLUMN, SqlDbType.DateTime2);
			where.AddParameter(query.CreatedBy, CREATED_BY_COLUMN, SqlDbType.VarChar);
			where.AddParameter(query.ModificationDate, MODIFICATION_DATE_COLUMN, SqlDbType.DateTime2);
			where.AddParameter(query.ModifiedBy, MODIFIED_BY_COLUMN, SqlDbType.VarChar);

			where.AddParameter(query.IctsReference, ICTS_REFERENCE_COLUMN, SqlDbType.VarChar);
			where.AddParameter(query.Tag, TAG_COLUMN, SqlDbType.VarChar);
			where.AddParameter(query.Serial, SERIAL_COLUMN, SqlDbType.VarChar);
			where.AddParameter(query.ProductId, PRODUCT_ID_COLUMN, SqlDbType.BigInt);
			where.AddParameter(query.Description, DESCRIPTION_COLUMN, SqlDbType.VarChar);
			where.AddParameter(query.InvoiceDate, INVOICE_DATE_COLUMN, SqlDbType.DateTime2);
			where.AddParameter(query.InvoiceNumber, INVOICE_NUMBER_COLUMN, SqlDbType.VarChar);
			where.AddParameter(query.Price, PRICE_COLUMN, SqlDbType.Decimal);
			where.AddParameter(query.PaidBy, PAID_BY_COLUMN, SqlDbType.VarChar);
			where.AddParameter(query.Owner, OWNER_COLUMN, SqlDbType.VarChar);
			where.AddParameter(query.InstallDate, INSTALL_DATE_COLUMN, SqlDbType.DateTime2);
			where.AddParameter(query.InstalledBy, INSTALLED_BY_COLUMN, SqlDbType.VarChar);
			where.AddParameter(query.Remark, REMARK_COLUMN, SqlDbType.VarChar);
			where.AddParameter(query.TeamAsset, TEAMASSET_COLUMN, SqlDbType.Bit);

			sb.Append(where.Build());
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
		private SqlCommand BuildSelectCommand(
			SqlConnection con,
			SqlTransaction tx,
			AssetQuery query = null,
			PageInfo pageInfo = null)
		{
			var cmd = new SqlCommand { Connection = con, Transaction = tx };

			var sb = new StringBuilder();
			sb.AppendLine(CreateSelectStatement());

			if (query != null)
				AddWhereClause(cmd.Parameters, sb, query);

			if (pageInfo != null)
				AddPaging(cmd.Parameters, sb, pageInfo);

			cmd.CommandText = sb.ToString();

			return cmd;
		}

		private SqlCommand BuildSelectCountCommand(
			SqlConnection con,
			SqlTransaction tx,
			AssetQuery query = null)
		{
			var cmd = new SqlCommand { Connection = con, Transaction = tx };

			var sb = new StringBuilder();
			sb.AppendLine(CreateSelectCountStatement());

			if (query != null)
				AddWhereClause(cmd.Parameters, sb, query);

			cmd.CommandText = sb.ToString();

			return cmd;
		}

		private void LinkProductsTo(IEnumerable<AssetDto> assets)
		{
			var productIds = assets.Select(x => x.Product.Id.Value).Distinct();
			var products = _productRepo.GetById(productIds);

			foreach (var asset in assets)
				asset.Product = products.Single(x => x.Id == asset.Product.Id);
		}

		public AssetDto GetById(long id) => GetById(new[] { id }).Single();

		public IEnumerable<AssetDto> GetById(IEnumerable<long> ids)
		{
			if (ids == null || !ids.Any())
				throw new ArgumentNullException("ids");

			// Filter out possible duplicates.
			var distinctIds = ids.Distinct().ToArray();

			IList<AssetDto> assets;
			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = BuildSelectCountCommand(con, tx, distinctIds))
					{
						long count = Convert.ToInt64(cmd.ExecuteScalar());
						if (distinctIds.Length != count)
							throw new ArgumentException("Not all supplied id's exist.");
					}

					using (var cmd = BuildSelectCommand(con, tx, distinctIds))
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

		public IEnumerable<AssetDto> Query(AssetQuery query = null)
		{
			IList<AssetDto> assets;
			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = BuildSelectCommand(con, tx, query))
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

		public PagedResult<AssetDto> PagedQuery(PageInfo pageInfo, AssetQuery query = null)
		{
			if (pageInfo == null)
				throw new ArgumentNullException("pageInfo");

			if (!pageInfo.OrderBy.IsValidFor(COLUMNS))
				throw new ArgumentException($"'{pageInfo.OrderBy.Name}' is an unrecognized column.");

			long totalCount;
			IList<AssetDto> assets;
			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = BuildSelectCountCommand(con, tx, query))
					{
						totalCount = Convert.ToInt64(cmd.ExecuteScalar());
					}

					using (var cmd = BuildSelectCommand(con, tx, query, pageInfo))
					using (var reader = cmd.ExecuteReader())
					{
						assets = ReadDtosFrom(reader).ToList();
					}

					//tx.Commit();
				}
			}

			LinkProductsTo(assets);

			return new PagedResult<AssetDto>(assets, pageInfo, totalCount);
		}

		public long Insert(AssetDto item)
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

			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText = 
						$@"INSERT INTO [{TableName}] 
							([{CREATION_DATE_COLUMN}], [{CREATED_BY_COLUMN}], [{MODIFICATION_DATE_COLUMN}], [{MODIFIED_BY_COLUMN}], 
							[{ICTS_REFERENCE_COLUMN}], [{TAG_COLUMN}], [{SERIAL_COLUMN}], [{PRODUCT_ID_COLUMN}], [{DESCRIPTION_COLUMN}],
							[{INVOICE_DATE_COLUMN}], [{INVOICE_NUMBER_COLUMN}], [{PRICE_COLUMN}], [{PAID_BY_COLUMN}], [{OWNER_COLUMN}], 
							[{INSTALL_DATE_COLUMN}], [{INSTALLED_BY_COLUMN}], [{REMARK_COLUMN}], [{TEAMASSET_COLUMN}], [{DELETED_COLUMN}])
						VALUES (@{CREATION_DATE_COLUMN}, @{CREATED_BY_COLUMN}, @{MODIFICATION_DATE_COLUMN}, @{MODIFIED_BY_COLUMN}, 
							@{ICTS_REFERENCE_COLUMN}, @{TAG_COLUMN}, @{SERIAL_COLUMN}, @{PRODUCT_ID_COLUMN}, @{DESCRIPTION_COLUMN}, 
							@{INVOICE_DATE_COLUMN}, @{INVOICE_NUMBER_COLUMN}, @{PRICE_COLUMN}, @{PAID_BY_COLUMN}, @{OWNER_COLUMN}, 
							@{INSTALL_DATE_COLUMN}, @{INSTALLED_BY_COLUMN}, @{REMARK_COLUMN}, @{TEAMASSET_COLUMN}, @{DELETED_COLUMN})
						SELECT CONVERT(bigint, SCOPE_IDENTITY())";

					long newId;
					using (var cmd = new SqlCommand(cmdText, con, tx))
					{
						AddSqlParameters(cmd.Parameters, item);
						cmd.Parameters.Add($"@{DELETED_COLUMN}", SqlDbType.Bit).Value = 0;

						newId = (long)cmd.ExecuteScalar();
					}

					tx.Commit();

					return newId;
				}
			}
		}

		public void Update(AssetDto item) => Update(new[] { item });

		public void Update(IEnumerable<AssetDto> items)
		{
			if (items == null || !items.Any())
				throw new ArgumentNullException("items");

			foreach (var item in items)
			{
				if (!item.Id.HasValue)
					throw new ArgumentException("Id is a required field.");
				if (string.IsNullOrWhiteSpace(item.ModifiedBy))
					throw new ArgumentException("ModifiedBy is required!");
				if (item.Product == null || !item.Product.Id.HasValue)
					throw new ArgumentException("Product Id is required!");
			}

			var now = DateTimeProvider.Now();

			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText =
						$@"UPDATE [{TableName}] SET
							[{MODIFICATION_DATE_COLUMN}]=@{MODIFICATION_DATE_COLUMN}, [{MODIFIED_BY_COLUMN}]=@{MODIFIED_BY_COLUMN}, 
							[{ICTS_REFERENCE_COLUMN}]=@{ICTS_REFERENCE_COLUMN}, [{TAG_COLUMN}]=@{TAG_COLUMN}, [{SERIAL_COLUMN}]=@{SERIAL_COLUMN}, 
							[{PRODUCT_ID_COLUMN}]=@{PRODUCT_ID_COLUMN}, [{DESCRIPTION_COLUMN}]=@{DESCRIPTION_COLUMN}, 
							[{INVOICE_DATE_COLUMN}]=@{INVOICE_DATE_COLUMN}, [{INVOICE_NUMBER_COLUMN}]=@{INVOICE_NUMBER_COLUMN}, 
							[{PRICE_COLUMN}]=@{PRICE_COLUMN}, [{PAID_BY_COLUMN}]=@{PAID_BY_COLUMN}, [{OWNER_COLUMN}]=@{OWNER_COLUMN}, 
							[{INSTALL_DATE_COLUMN}]=@{INSTALL_DATE_COLUMN}, [{INSTALLED_BY_COLUMN}]=@{INSTALLED_BY_COLUMN}, 
							[{REMARK_COLUMN}]=@{REMARK_COLUMN}, [{TEAMASSET_COLUMN}]=@{TEAMASSET_COLUMN}
						WHERE [{ID_COLUMN}]=@{ID_COLUMN} AND [{DELETED_COLUMN}]=@{DELETED_COLUMN}";

					foreach (var item in items)
					{
						item.ModificationDate = now;

						using (var cmd = new SqlCommand(cmdText, con, tx))
						{
							AddSqlParameters(cmd.Parameters, item);
							cmd.Parameters.Add($"@{DELETED_COLUMN}", SqlDbType.Bit).Value = 0;

							cmd.ExecuteNonQuery();
						}
					}

					tx.Commit();
				}
			}
		}
	}
}