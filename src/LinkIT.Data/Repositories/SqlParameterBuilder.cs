using System;
using System.Data;
using System.Data.SqlClient;

namespace LinkIT.Data.Repositories
{
	public class SqlParameterBuilder
	{
		private readonly SqlParameterCollection _params;

		public SqlParameterBuilder(SqlParameterCollection @params)
		{
			_params = @params;
		}

		public void Add<T>(T value, string columnName, SqlDbType sqlType, bool addIfNull = false)
		{
			string paramName = $"@{columnName}";
			if (value == null && addIfNull)
			{
				_params.Add(paramName, sqlType).Value = DBNull.Value;

				return;
			}

			if (value == null)
				return;

			_params.Add(paramName, sqlType).Value = value;
		}
	}
}