using System;
using System.Data;
using System.Data.SqlClient;

namespace LinkIT.Data.Repositories
{
	public class SqlParameterBuilder
	{
		public SqlParameterBuilder(SqlParameterCollection @params)
		{
			Parameters = @params;
		}

		public SqlParameterCollection Parameters { get; private set; }

		protected void Add<T>(T value, string columnName, SqlDbType sqlType, bool addIfNull)
		{
			string paramName = $"@{columnName}";
			if (value == null && addIfNull)
			{
				Parameters.Add(paramName, sqlType).Value = DBNull.Value;

				return;
			}

			if (value == null)
				return;

			Parameters.Add(paramName, sqlType).Value = value;
		}

		public void Add<T>(T value, string columnName, SqlDbType sqlType)
		{
			Add(value, columnName, sqlType, true);
		}
	}
}