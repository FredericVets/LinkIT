using LinkIT.Data.DTO;
using LinkIT.Data.Extensions;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using System;
using System.Data;
using System.Text;

namespace LinkIT.Data.Builders
{
	public class WhereClauseBuilder
	{
		private readonly IDbCommand _command;
		private readonly LogicalOperator _logicalOperator;
		private readonly bool _hasSoftDelete;
		private readonly StringBuilder _builder;
		private bool _isFirstParameter;

		public WhereClauseBuilder(IDbCommand command, LogicalOperator logicalOperator, bool hasSoftDelete)
		{
			_command = command ?? throw new ArgumentNullException("command");
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
			_command.AddSqlParameter(paramName, value, sqlType);
			_isFirstParameter = false;
		}

		public override string ToString() => _builder.ToString();
	}
}