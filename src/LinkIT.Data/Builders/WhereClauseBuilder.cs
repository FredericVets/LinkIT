using LinkIT.Data.DTO;
using LinkIT.Data.Queries;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace LinkIT.Data.Repositories
{
	public class WhereClauseBuilder
	{
		private readonly SqlParameterCollection _params;
		private readonly LogicalOperator _logicalOperator;
		private readonly bool _hasSoftDelete;
		private readonly StringBuilder _builder;
		private bool _isFirstParameter;

		public WhereClauseBuilder(SqlParameterCollection @params, LogicalOperator logicalOperator, bool hasSoftDelete)
		{
			if (@params == null)
				throw new ArgumentNullException("params");

			_params = @params;
			_logicalOperator = logicalOperator;
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
				_isFirstParameter = false;

				return;
			}

			_builder.AppendLine("WHERE");
		}

		public void AddParameter<T>(T value, string columnName, SqlDbType sqlType)
		{
			if (value == null)
				return;

			if (!_isFirstParameter)
				_builder.AppendLine(_logicalOperator.ToString());

			string paramName = $"@{columnName}";
			_builder.AppendLine($"[{columnName}] = {paramName}");
			_params.Add(paramName, sqlType).Value = value;
			_isFirstParameter = false;
		}

		public override string ToString() => _builder.ToString();
	}
}