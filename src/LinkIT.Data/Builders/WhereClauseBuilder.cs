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

		public WhereClauseBuilder(IDbCommand command, bool hasSoftDelete) : 
			this(command, LogicalOperator.AND, hasSoftDelete) { }

		public WhereClauseBuilder(IDbCommand command, LogicalOperator logicalOperator, bool hasSoftDelete)
		{
			_command = command ?? throw new ArgumentNullException(nameof(command));
			_logicalOperator = logicalOperator;
			_hasSoftDelete = hasSoftDelete;
			_builder = new StringBuilder();

			_isFirstParameter = true;
		}

		public WhereClauseBuilder ForParameter<T>(T value, string columnName, SqlDbType sqlType)
		{
			if (value == null)
				return this;

			if (!_isFirstParameter)
				_builder.AppendLine(_logicalOperator.ToString());

			if (_isFirstParameter)
			{
				_builder.AppendLine("WHERE");
				_isFirstParameter = false;
			}

			string paramName = $"@{columnName}";
			_builder.AppendLine($"[{columnName}] = {paramName}");
			_command.AddSqlParameter(paramName, value, sqlType);

			return this;
		}

		public override string ToString()
		{
			if (_hasSoftDelete && _isFirstParameter)
			{
				_builder.AppendLine("WHERE");
				_builder.AppendLine($"[{Repository<Dto, Query>.DELETED_COLUMN}] = 0");

				return _builder.ToString();
			}

			if (_hasSoftDelete)
			{
				_builder.AppendLine(LogicalOperator.AND.ToString());
				_builder.AppendLine($"[{Repository<Dto, Query>.DELETED_COLUMN}] = 0");

				return _builder.ToString();
			}

			return _builder.ToString();
		}
	}
}