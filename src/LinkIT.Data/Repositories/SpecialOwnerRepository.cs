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
	public class SpecialOwnerRepository : Repository<SpecialOwnerDto, SpecialOwnerQuery>
	{
		public const string NAME_COLUMN = "Name";
		public const string REMARK_COLUMN = "Remark";

		public static readonly string[] COLUMNS = new[]
		{
			ID_COLUMN, CREATION_DATE_COLUMN, CREATED_BY_COLUMN, MODIFICATION_DATE_COLUMN, MODIFIED_BY_COLUMN, NAME_COLUMN, REMARK_COLUMN
		};

		public SpecialOwnerRepository(string connectionString) : base(connectionString, TableNames.SPECIAL_OWNER_TABLE) { }

		protected override IEnumerable<string> Columns => COLUMNS;

		protected override IEnumerable<SpecialOwnerDto> ReadDtosFrom(SqlDataReader reader)
		{
			while (reader.Read())
			{
				yield return new SpecialOwnerDto
				{
					Id = GetColumnValue<long?>(reader, ID_COLUMN),
					CreationDate = GetColumnValue<DateTime?>(reader, CREATION_DATE_COLUMN),
					CreatedBy = GetColumnValue<string>(reader, CREATED_BY_COLUMN),
					ModificationDate = GetColumnValue<DateTime?>(reader, MODIFICATION_DATE_COLUMN),
					ModifiedBy = GetColumnValue<string>(reader, MODIFIED_BY_COLUMN),
					Name = GetColumnValue<string>(reader, NAME_COLUMN),
					Remark = GetColumnValue<string>(reader, REMARK_COLUMN)
				};
			}
		}

		protected override void AddWhereClause(SqlParameterCollection @params, StringBuilder sb, SpecialOwnerQuery query)
		{
			var where = new WhereClauseBuilder(@params, query.LogicalOperator, false);
			where.AddParameter(query.Id, ID_COLUMN, SqlDbType.BigInt);
			where.AddParameter(query.CreationDate, CREATION_DATE_COLUMN, SqlDbType.DateTime2);
			where.AddParameter(query.CreatedBy, CREATED_BY_COLUMN, SqlDbType.VarChar);
			where.AddParameter(query.ModificationDate, MODIFICATION_DATE_COLUMN, SqlDbType.BigInt);
			where.AddParameter(query.ModifiedBy, MODIFIED_BY_COLUMN, SqlDbType.DateTime2);
			where.AddParameter(query.Name, NAME_COLUMN, SqlDbType.VarChar);
			where.AddParameter(query.Remark, REMARK_COLUMN, SqlDbType.VarChar);

			sb.Append(where.Build());
		}

		protected override string CreateInsertStatement()
		{
			return $@"INSERT INTO [{TableName}] 
						([{CREATION_DATE_COLUMN}], [{CREATED_BY_COLUMN}], [{MODIFICATION_DATE_COLUMN}], [{MODIFIED_BY_COLUMN}], 
						[{NAME_COLUMN}], [{REMARK_COLUMN}]) 
					VALUES (@{CREATION_DATE_COLUMN}, @{CREATED_BY_COLUMN}, @{MODIFICATION_DATE_COLUMN}, @{MODIFIED_BY_COLUMN}, 
						@{NAME_COLUMN}, @{REMARK_COLUMN})";
		}

		protected override string CreateUpdateStatement()
		{
			return $@"UPDATE [{TableName}] SET 
						[{MODIFICATION_DATE_COLUMN}]=@{MODIFICATION_DATE_COLUMN}, [{MODIFIED_BY_COLUMN}]=@{MODIFIED_BY_COLUMN},
						[{NAME_COLUMN}]=@{NAME_COLUMN}, [{REMARK_COLUMN}]=@{REMARK_COLUMN}
					WHERE [{ID_COLUMN}]=@{ID_COLUMN}";
		}

		protected override void AddSqlParameters(SqlParameterCollection @params, SpecialOwnerDto input)
		{
			var paramBuilder = new SqlParameterBuilder(@params);
			paramBuilder.Add(input.Id, ID_COLUMN, SqlDbType.BigInt);
			paramBuilder.Add(input.CreationDate, CREATION_DATE_COLUMN, SqlDbType.DateTime2);
			paramBuilder.Add(input.CreatedBy, CREATED_BY_COLUMN, SqlDbType.VarChar);
			paramBuilder.Add(input.ModificationDate, MODIFICATION_DATE_COLUMN, SqlDbType.DateTime2);
			paramBuilder.Add(input.ModifiedBy, MODIFIED_BY_COLUMN, SqlDbType.VarChar);

			paramBuilder.Add(input.Name, NAME_COLUMN, SqlDbType.VarChar);
			paramBuilder.Add(input.Remark, REMARK_COLUMN, SqlDbType.VarChar);
		}

		public override long Insert(SpecialOwnerDto item)
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

		public override void Update(IEnumerable<SpecialOwnerDto> items)
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