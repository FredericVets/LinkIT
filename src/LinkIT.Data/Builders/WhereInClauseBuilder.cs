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
	public class WhereInClauseBuilder
	{
		private readonly string _columnName;
		private readonly SqlParameterCollection _params;
		private readonly bool _hasSoftDelete;
		private readonly StringBuilder _builder;
		private bool _isFirstParameter;

		public WhereInClauseBuilder(string columnName, SqlParameterCollection @params, bool hasSoftDelete)
		{
			if (string.IsNullOrWhiteSpace(columnName))
				throw new ArgumentNullException(columnName);
			if (@params == null)
				throw new ArgumentNullException("params");

			_columnName = columnName;
			_params = @params;
			_hasSoftDelete = hasSoftDelete;
			_builder = new StringBuilder();
			_isFirstParameter = true;

			Initialize();
		}

		private void Initialize()
		{
			_builder.Append($"WHERE [{_columnName}] IN (");
		}

		public void AddParameters<T>(IEnumerable<T> values, SqlDbType sqlType)
		{
			if (values == null || !values.Any())
				throw new ArgumentNullException("values");

			for (int i = 0; i < values.Count(); i++)
			{
				if (!_isFirstParameter)
					_builder.Append(", ");

				string identifier = $"@Value{i}";
				_builder.Append(identifier);
				_params.Add(identifier, sqlType).Value = values.ElementAt(i);

				_isFirstParameter = false;
			}

			_builder.AppendLine(")");
		}

		public override string ToString()
		{
			if (!_hasSoftDelete)
				return _builder.ToString();

			_builder.AppendLine($"AND [{Repository<Dto, Query>.DELETED_COLUMN}] = 0");

			return _builder.ToString();
		}
	}
}