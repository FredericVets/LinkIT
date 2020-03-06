using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace LinkIT.Data.Repositories
{
	/// <summary>
	/// The base repository for all Dto's.
	/// </summary>
	public abstract class Repository : IRepository
	{
		public const string ID_COLUMN = "Id";

		public Repository(string connectionString, string tableName)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
				throw new ArgumentNullException("connectionString");
			if (string.IsNullOrWhiteSpace(tableName))
				throw new ArgumentNullException("tableName");

			ConnectionString = connectionString;
			TableName = tableName;
		}

		protected string TableName { get; }

		protected string ConnectionString { get; }

		protected static void AddWhereClause(SqlParameterCollection @params, StringBuilder sb, long[] ids)
		{
			sb.Append($"WHERE [{ID_COLUMN}] IN (");

			bool first = true;
			for (int i = 0; i < ids.Length; i++)
			{
				if (!first)
					sb.Append(", ");

				string identifier = $"@Id{i}";
				sb.Append(identifier);
				@params.Add(identifier, SqlDbType.BigInt).Value = ids[i];

				first = false;
			}

			sb.AppendLine(")");
		}

		protected SqlCommand CreateSelectCommand(
			SqlConnection con,
			SqlTransaction tx,
			long[] ids)
		{
			var cmd = new SqlCommand { Connection = con, Transaction = tx };

			var sb = new StringBuilder();
			sb.AppendLine($"SELECT * FROM [{TableName}]");

			AddWhereClause(cmd.Parameters, sb, ids);

			cmd.CommandText = sb.ToString();

			return cmd;
		}

		protected SqlCommand CreateSelectCountCommand(
			SqlConnection con,
			SqlTransaction tx,
			long[] ids)
		{
			var cmd = new SqlCommand { Connection = con, Transaction = tx };

			var sb = new StringBuilder();
			sb.AppendLine($"SELECT COUNT({ID_COLUMN}) FROM [{TableName}]");

			AddWhereClause(cmd.Parameters, sb, ids);

			cmd.CommandText = sb.ToString();

			return cmd;
		}

		public bool Exists(long id)
		{
			return Exists(new[] { id });
		}

		public bool Exists(IEnumerable<long> ids)
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

						return distinctIds.Length == count;
					}

					//tx.Commit();
				}
			}
		}

		public void Delete(long id)
		{
			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText = $"DELETE FROM [{TableName}] WHERE [{ID_COLUMN}]=@Id";

					using (var cmd = new SqlCommand(cmdText, con, tx))
					{
						cmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = id;

						cmd.ExecuteNonQuery();
					}

					tx.Commit();
				}
			}
		}
	}
}