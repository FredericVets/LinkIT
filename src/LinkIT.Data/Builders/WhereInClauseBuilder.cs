using LinkIT.Data.DTO;
using LinkIT.Data.Extensions;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LinkIT.Data.Builders
{
	public class WhereInClauseBuilder
	{
		private readonly string _columnName;
		private readonly IDbCommand _command;
		private readonly bool _hasSoftDelete;
		private readonly StringBuilder _builder;
		private bool _isFirstParameter;

		public WhereInClauseBuilder(string columnName, IDbCommand command, bool hasSoftDelete)
		{
			if (string.IsNullOrWhiteSpace(columnName))
				throw new ArgumentNullException(columnName);

			_columnName = columnName;
			_command = command ?? throw new ArgumentNullException("command");
			_hasSoftDelete = hasSoftDelete;
			_builder = new StringBuilder();
			_isFirstParameter = true;

			Initialize();
		}

		private void Initialize()
		{
			if (_hasSoftDelete)
			{
				_builder.AppendLine($"WHERE [{Repository<Dto, Query>.DELETED_COLUMN}] = 0");
				_builder.Append($"AND [{_columnName}] IN (");

				return;
			}

			_builder.Append($"WHERE [{_columnName}] IN (");
		}

		public WhereInClauseBuilder ForParameters<T>(IEnumerable<T> values, SqlDbType sqlType)
		{
			if (values == null || !values.Any())
				throw new ArgumentNullException("values");

			var valuesArray = values.ToArray();
			for (int i = 0; i < valuesArray.Length; i++)
			{
				if (!_isFirstParameter)
					_builder.Append(", ");

				string paramName = $"@Value{i}";
				_builder.Append(paramName);
				_command.AddSqlParameter(paramName, valuesArray[i], sqlType);

				_isFirstParameter = false;
			}

			_builder.AppendLine(")");

			return this;
		}

		public override string ToString() => 
			_builder.ToString();
	}
}