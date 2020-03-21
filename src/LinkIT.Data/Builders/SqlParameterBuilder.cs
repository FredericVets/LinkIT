using LinkIT.Data.Extensions;
using System;
using System.Data;

namespace LinkIT.Data.Builders
{
	public class SqlParameterBuilder
	{
		private readonly IDbCommand _command;

		public SqlParameterBuilder(IDbCommand command)
		{
			_command = command ?? throw new ArgumentNullException("command");
		}

		public void AddParameter<T>(T value, string columnName, SqlDbType sqlType)
		{
			string paramName = $"@{columnName}";
			if (value == null)
			{
				_command.AddSqlParameter(paramName, DBNull.Value, sqlType);

				return;
			}

			_command.AddSqlParameter(paramName, value, sqlType);
		}
	}
}