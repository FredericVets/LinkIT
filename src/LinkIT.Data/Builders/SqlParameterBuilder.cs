﻿using System;
using System.Data;
using System.Data.SqlClient;

namespace LinkIT.Data.Repositories
{
	public class SqlParameterBuilder
	{
		private readonly SqlParameterCollection _params;

		public SqlParameterBuilder(SqlParameterCollection @params)
		{
			if (@params == null)
				throw new ArgumentNullException("params");

			_params = @params;
		}

		public void AddParameter<T>(T value, string columnName, SqlDbType sqlType)
		{
			string paramName = $"@{columnName}";
			if (value == null)
			{
				_params.Add(paramName, sqlType).Value = DBNull.Value;

				return;
			}

			_params.Add(paramName, sqlType).Value = value;
		}
	}
}