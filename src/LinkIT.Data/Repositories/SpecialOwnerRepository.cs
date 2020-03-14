﻿using LinkIT.Data.DTO;
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
	public class SpecialOwnerRepository : Repository, IRepository<SpecialOwnerDto, SpecialOwnerQuery>
	{
		public const string NAME_COLUMN = "Name";
		public const string REMARK_COLUMN = "Remark";

		public static readonly string[] COLUMNS = new[]
		{
			ID_COLUMN, CREATION_DATE_COLUMN, CREATED_BY_COLUMN, MODIFICATION_DATE_COLUMN, MODIFIED_BY_COLUMN, NAME_COLUMN, REMARK_COLUMN
		};

		public SpecialOwnerRepository(string connectionString) : base(connectionString, TableNames.SPECIAL_OWNER_TABLE) { }

		private static IEnumerable<SpecialOwnerDto> ReadDtosFrom(SqlDataReader reader)
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

		private static void AddSqlParameters(SqlParameterCollection @params, SpecialOwnerDto input)
		{
			var paramBuilder = new SqlParameterBuilder(@params);
			paramBuilder.Add(input.Id, ID_COLUMN, SqlDbType.BigInt);
			paramBuilder.Add(input.CreationDate, CREATION_DATE_COLUMN, SqlDbType.DateTime2);
			paramBuilder.Add(input.CreatedBy, CREATED_BY_COLUMN, SqlDbType.VarChar);
			paramBuilder.Add(input.ModificationDate, MODIFICATION_DATE_COLUMN, SqlDbType.DateTime2);
			paramBuilder.Add(input.ModifiedBy, MODIFIED_BY_COLUMN, SqlDbType.VarChar);

			paramBuilder.Add(input.Name, NAME_COLUMN, SqlDbType.VarChar, true);
			paramBuilder.Add(input.Remark, REMARK_COLUMN, SqlDbType.VarChar, true);
		}

		private static void AddWhereClause(SqlParameterCollection @params, StringBuilder sb, SpecialOwnerQuery query)
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
			SpecialOwnerQuery query = null,
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

		private SqlCommand CreateSelectCountCommand(
			SqlConnection con,
			SqlTransaction tx,
			SpecialOwnerQuery query = null)
		{
			var cmd = new SqlCommand { Connection = con, Transaction = tx };

			var sb = new StringBuilder();
			sb.AppendLine(CreateSelectCountStatement());

			if (query != null)
				AddWhereClause(cmd.Parameters, sb, query);

			cmd.CommandText = sb.ToString();

			return cmd;
		}

		public SpecialOwnerDto GetById(long id) => GetById(new[] { id }).Single();

		public IEnumerable<SpecialOwnerDto> GetById(IEnumerable<long> ids)
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
						return ReadDtosFrom(reader).ToList();
					}

					//tx.Commit();
				}
			}
		}

		public IEnumerable<SpecialOwnerDto> Query(SpecialOwnerQuery query = null)
		{
			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = CreateSelectCommand(con, tx, query))
					using (var reader = cmd.ExecuteReader())
					{
						return ReadDtosFrom(reader).ToList();
					}

					//tx.Commit();
				}
			}
		}

		public PagedResult<SpecialOwnerDto> PagedQuery(PageInfo pageInfo, SpecialOwnerQuery query = null)
		{
			if (pageInfo == null)
				throw new ArgumentNullException("pageInfo");

			if (!pageInfo.OrderBy.IsValidFor(COLUMNS))
				throw new ArgumentException($"'{pageInfo.OrderBy.Name}' is an unrecognized column.");

			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					long totalCount;
					using (var cmd = CreateSelectCountCommand(con, tx, query))
					{
						totalCount = Convert.ToInt64(cmd.ExecuteScalar());
					}

					using (var cmd = CreateSelectCommand(con, tx, query, pageInfo))
					using (var reader = cmd.ExecuteReader())
					{
						var result = ReadDtosFrom(reader).ToList();

						return new PagedResult<SpecialOwnerDto>(result, pageInfo, totalCount);
					}

					//tx.Commit();
				}
			}
		}

		public long Insert(SpecialOwnerDto item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			if (item.Id.HasValue)
				throw new ArgumentException("Id can not be specified.");

			if (string.IsNullOrWhiteSpace(item.CreatedBy))
				throw new ArgumentException("CreatedBy is required!");

			item.ModifiedBy = item.CreatedBy;
			item.ModificationDate = item.CreationDate = DateTimeProvider.Now();

			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText = $@"INSERT INTO [{TableName}] ([{CREATION_DATE_COLUMN}], [{CREATED_BY_COLUMN}], [{MODIFICATION_DATE_COLUMN}], [{MODIFIED_BY_COLUMN}], [{NAME_COLUMN}], [{REMARK_COLUMN}]) 
						VALUES (@{CREATION_DATE_COLUMN}, @{CREATED_BY_COLUMN}, @{MODIFICATION_DATE_COLUMN}, @{MODIFIED_BY_COLUMN}, @{NAME_COLUMN}, @{REMARK_COLUMN})
						SELECT CONVERT(bigint, SCOPE_IDENTITY())";

					long newId;
					using (var cmd = new SqlCommand(cmdText, con, tx))
					{
						AddSqlParameters(cmd.Parameters, item);

						newId = (long)cmd.ExecuteScalar();
					}

					tx.Commit();

					return newId;
				}
			}
		}

		public void Update(SpecialOwnerDto item) => Update(new[] { item });

		public void Update(IEnumerable<SpecialOwnerDto> items)
		{
			if (items == null || !items.Any())
				throw new ArgumentNullException("items");

			foreach (var item in items)
			{
				if (!item.Id.HasValue)
					throw new ArgumentException("Id is a required field.");
				if (string.IsNullOrWhiteSpace(item.ModifiedBy))
					throw new ArgumentException("ModifiedBy is required!");
			}

			var now = DateTimeProvider.Now();

			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText = $@"UPDATE [{TableName}] 
						SET [{MODIFICATION_DATE_COLUMN}]=@{MODIFICATION_DATE_COLUMN}, [{MODIFIED_BY_COLUMN}]=@{MODIFIED_BY_COLUMN},
							[{NAME_COLUMN}]=@{NAME_COLUMN}, [{REMARK_COLUMN}]=@{REMARK_COLUMN}
						WHERE [{ID_COLUMN}]=@{ID_COLUMN}";

					foreach (var item in items)
					{
						item.ModificationDate = now;

						using (var cmd = new SqlCommand(cmdText, con, tx))
						{
							AddSqlParameters(cmd.Parameters, item);

							cmd.ExecuteNonQuery();
						}
					}

					tx.Commit();
				}
			}
		}
	}
}