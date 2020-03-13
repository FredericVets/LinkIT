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

		public AssetRepository(string connectionString) : base(connectionString, TableNames.ASSET_TABLE, hasSoftDelete: true) { }

		private static IEnumerable<AssetDto> ReadDtosFrom(SqlDataReader reader)
		{
			while (reader.Read())
			{
				yield return new AssetDto
				{
					Id = GetValue<long?>(reader, ID_COLUMN),
					CreationDate = GetValue<DateTime?>(reader, CREATION_DATE_COLUMN),
					CreatedBy = GetValue<string>(reader, CREATED_BY_COLUMN),
					ModificationDate = GetValue<DateTime?>(reader, MODIFICATION_DATE_COLUMN),
					ModifiedBy = GetValue<string>(reader, MODIFIED_BY_COLUMN),
					IctsReference = GetValue<string>(reader, ICTS_REFERENCE_COLUMN),
					Tag = GetValue<string>(reader, TAG_COLUMN),
					Serial = GetValue<string>(reader, SERIAL_COLUMN),
					Product = new ProductDto
					{
						Id = GetValue<long?>(reader, PRODUCT_ID_COLUMN)
					},
					Description = GetValue<string>(reader, DESCRIPTION_COLUMN),
					InvoiceDate = GetValue<DateTime?>(reader, INVOICE_DATE_COLUMN),
					InvoiceNumber = GetValue<string>(reader, INVOICE_NUMBER_COLUMN),
					Price = GetValue<decimal?>(reader, PRICE_COLUMN),
					PaidBy = GetValue<string>(reader, PAID_BY_COLUMN),
					Owner = GetValue<string>(reader, OWNER_COLUMN),
					InstallDate = GetValue<DateTime?>(reader, INSTALL_DATE_COLUMN),
					InstalledBy = GetValue<string>(reader, INSTALLED_BY_COLUMN),
					Remark = GetValue<string>(reader, REMARK_COLUMN),
					TeamAsset = GetValue<bool?>(reader, TEAMASSET_COLUMN)
				};
			}
		}

		private static void AddWhereClause(SqlParameterCollection @params, StringBuilder sb, AssetQuery query)
		{
			sb.AppendLine($"WHERE [{DELETED_COLUMN}] = 0");

			AddSqlParameter(query.Id, ID_COLUMN, SqlDbType.BigInt);			
			AddSqlParameter(query.CreationDate, CREATION_DATE_COLUMN, SqlDbType.DateTime2);
			AddSqlParameter(query.CreatedBy, CREATED_BY_COLUMN, SqlDbType.VarChar);
			AddSqlParameter(query.ModificationDate, MODIFICATION_DATE_COLUMN, SqlDbType.DateTime2);
			AddSqlParameter(query.ModifiedBy, MODIFIED_BY_COLUMN, SqlDbType.VarChar);
			AddSqlParameter(query.IctsReference, ICTS_REFERENCE_COLUMN, SqlDbType.VarChar);
			AddSqlParameter(query.Tag, TAG_COLUMN, SqlDbType.VarChar);
			AddSqlParameter(query.Serial, SERIAL_COLUMN, SqlDbType.VarChar);
			AddSqlParameter(query.ProductId, PRODUCT_ID_COLUMN, SqlDbType.BigInt);
			AddSqlParameter(query.Description, DESCRIPTION_COLUMN, SqlDbType.VarChar);
			AddSqlParameter(query.InvoiceDate, INVOICE_DATE_COLUMN, SqlDbType.DateTime2);
			AddSqlParameter(query.InvoiceNumber, INVOICE_NUMBER_COLUMN, SqlDbType.VarChar);
			AddSqlParameter(query.Price, PRICE_COLUMN, SqlDbType.Decimal);
			AddSqlParameter(query.PaidBy, PAID_BY_COLUMN, SqlDbType.VarChar);
			AddSqlParameter(query.Owner, OWNER_COLUMN, SqlDbType.VarChar);
			AddSqlParameter(query.InstallDate, INSTALL_DATE_COLUMN, SqlDbType.DateTime2);
			AddSqlParameter(query.InstalledBy, INSTALLED_BY_COLUMN, SqlDbType.VarChar);
			AddSqlParameter(query.Remark, REMARK_COLUMN, SqlDbType.VarChar);
			AddSqlParameter(query.TeamAsset, TEAMASSET_COLUMN, SqlDbType.Bit);

			void AddSqlParameter<T>(T value, string columnName, SqlDbType sqlType)
			{
				if (value == null)
					return;

				sb.AppendLine(query.LogicalOperator.ToString());
				sb.AppendLine($"[{columnName}] = @{columnName}");
				@params.Add($"@{columnName}", sqlType).Value = value;
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
		private SqlCommand CreateSelectCommand(
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

		public AssetDto GetById(long id) => GetById(new[] { id }).Single();

		public IEnumerable<AssetDto> GetById(IEnumerable<long> ids)
		{
			if (ids == null || !ids.Any())
				throw new ArgumentNullException("ids");

			// Filter out possible duplicates.
			var distinctIds = ids.Distinct().ToArray();

			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = CreateSelectCountCommand(con, tx, distinctIds))
					{
						long count = Convert.ToInt64(cmd.ExecuteScalar());
						if (distinctIds.Length != count)
							throw new ArgumentException("Not all supplied id's exist.");
					}

					using (var cmd = CreateSelectCommand(con, tx, distinctIds))
					using (var reader = cmd.ExecuteReader())
					{
						// TODO : fetch the product as well.
						return ReadDtosFrom(reader).ToList();
					}

					//tx.Commit();
				}
			}
		}

		public IEnumerable<AssetDto> Query(AssetQuery query = null)
		{
			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = CreateSelectCommand(con, tx, query))
					using (var reader = cmd.ExecuteReader())
					{
						// TODO : fetch the product as well.
						return ReadDtosFrom(reader).ToList();
					}

					//tx.Commit();
				}
			}
		}

		public PagedResult<AssetDto> PagedQuery(PageInfo paging, AssetQuery query = null)
		{
			throw new NotImplementedException();
		}

		public long Insert(AssetDto item)
		{
			throw new NotImplementedException();
		}

		public void Update(AssetDto item) => Update(new[] { item });

		public void Update(IEnumerable<AssetDto> data)
		{
			throw new NotImplementedException();
		}
	}
}