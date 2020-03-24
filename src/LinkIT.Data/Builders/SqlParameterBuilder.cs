using LinkIT.Data.Extensions;
using System;
using System.Data;

namespace LinkIT.Data.Builders
{
	public class SqlParameterBuilder
	{
		private readonly IDbCommand _command;

		public SqlParameterBuilder(IDbCommand command) =>
			_command = command ?? throw new ArgumentNullException("command");

		public SqlParameterBuilder ForParameter<T>(T value, string columnName, SqlDbType sqlType)
		{
			string paramName = $"@{columnName}";
			if (value == null)
			{
				_command.AddSqlParameter(paramName, DBNull.Value, sqlType);

				return this;
			}

			_command.AddSqlParameter(paramName, value, sqlType);

			return this;
		}
	}
}