using LinkIT.Data.Builders;
using LinkIT.Data.DTO;
using LinkIT.Data.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LinkIT.Data.Repositories
{
	public class ProductRepository : Repository<ProductDto, ProductQuery>
	{
		public const string BRAND_COLUMN = "Brand";
		public const string TYPE_COLUMN = "Type";

		private static readonly string[] COLUMNS = new[]
		{
			ID_COLUMN, CREATION_DATE_COLUMN, CREATED_BY_COLUMN, MODIFICATION_DATE_COLUMN, MODIFIED_BY_COLUMN, BRAND_COLUMN, TYPE_COLUMN
		};

		public ProductRepository(string connectionString) : base(connectionString, TableNames.PRODUCT_TABLE) { }

		protected override void AddParametersFor(ProductDto input, SqlParameterBuilder builder)
		{
			builder.AddParameter(input.Id, ID_COLUMN, SqlDbType.BigInt);
			builder.AddParameter(input.CreationDate, CREATION_DATE_COLUMN, SqlDbType.DateTime2);
			builder.AddParameter(input.CreatedBy, CREATED_BY_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.ModificationDate, MODIFICATION_DATE_COLUMN, SqlDbType.DateTime2);
			builder.AddParameter(input.ModifiedBy, MODIFIED_BY_COLUMN, SqlDbType.VarChar);

			builder.AddParameter(input.Brand, BRAND_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.Type, TYPE_COLUMN, SqlDbType.VarChar);
		}

		protected override void AddParametersFor(ProductQuery input, WhereClauseBuilder builder)
		{
			builder.AddParameter(input.Id, ID_COLUMN, SqlDbType.BigInt);
			builder.AddParameter(input.CreationDate, CREATION_DATE_COLUMN, SqlDbType.DateTime2);
			builder.AddParameter(input.CreatedBy, CREATED_BY_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.ModificationDate, MODIFICATION_DATE_COLUMN, SqlDbType.DateTime2);
			builder.AddParameter(input.ModifiedBy, MODIFIED_BY_COLUMN, SqlDbType.VarChar);

			builder.AddParameter(input.Brand, BRAND_COLUMN, SqlDbType.VarChar);
			builder.AddParameter(input.Type, TYPE_COLUMN, SqlDbType.VarChar);
		}

		protected override IEnumerable<ProductDto> ReadDtosFrom(SqlDataReader reader)
		{
			while (reader.Read())
			{
				yield return new ProductDto
				{
					Id = GetColumnValue<long?>(reader, ID_COLUMN),
					CreationDate = GetColumnValue<DateTime?>(reader, CREATION_DATE_COLUMN),
					CreatedBy = GetColumnValue<string>(reader, CREATED_BY_COLUMN),
					ModificationDate = GetColumnValue<DateTime?>(reader, MODIFICATION_DATE_COLUMN),
					ModifiedBy = GetColumnValue<string>(reader, MODIFIED_BY_COLUMN),
					Brand = GetColumnValue<string>(reader, BRAND_COLUMN),
					Type = GetColumnValue<string>(reader, TYPE_COLUMN)
				};
			}
		}

		protected override string CreateInsertStatement()
		{
			return $@"INSERT INTO [{TableName}] 
						([{CREATION_DATE_COLUMN}], [{CREATED_BY_COLUMN}], [{MODIFICATION_DATE_COLUMN}], [{MODIFIED_BY_COLUMN}], 
						[{BRAND_COLUMN}], [{TYPE_COLUMN}]) 
					VALUES (@{CREATION_DATE_COLUMN}, @{CREATED_BY_COLUMN}, @{MODIFICATION_DATE_COLUMN}, @{MODIFIED_BY_COLUMN}, 
						@{BRAND_COLUMN}, @{TYPE_COLUMN})";
		}

		protected override string CreateUpdateStatement()
		{
			return $@"UPDATE [{TableName}] SET 
						[{MODIFICATION_DATE_COLUMN}]=@{MODIFICATION_DATE_COLUMN}, [{MODIFIED_BY_COLUMN}]=@{MODIFIED_BY_COLUMN}, 
						[{BRAND_COLUMN}]=@{BRAND_COLUMN}, [{TYPE_COLUMN}]=@{TYPE_COLUMN}
					WHERE [{ID_COLUMN}]=@{ID_COLUMN}";
		}

		public override IEnumerable<string> Columns => COLUMNS;

		public override long Insert(ProductDto item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			if (item.Id.HasValue)
				throw new ArgumentException("Id can not be specified.");
			if (string.IsNullOrWhiteSpace(item.CreatedBy))
				throw new ArgumentException("CreatedBy is required!");

			item.ModifiedBy = item.CreatedBy;
			item.ModificationDate = item.CreationDate = DateTimeProvider.Now();

			return base.Insert(item);
		}

		public override void Update(IEnumerable<ProductDto> items)
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

				item.ModificationDate = now;
			}

			base.Update(items);
		}
	}
}