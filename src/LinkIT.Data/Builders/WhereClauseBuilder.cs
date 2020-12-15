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

		private void HandleFirstParameter()
		{
			if (_isFirstParameter)
			{
				_builder.AppendLine("WHERE (");
				_isFirstParameter = false;

				return;
			}

			_builder.AppendLine(_logicalOperator.ToString());
		}

		public WhereClauseBuilder ForParameter<T>(T value, string columnName, SqlDbType sqlType)
		{
			if (value == null)
				return this;

			HandleFirstParameter();

			string paramName = $"@{columnName}";
			if (value is string)
			{
				_builder.AppendLine($"[{columnName}] like {paramName}");
				_command.AddSqlParameter(paramName, value, sqlType);

				return this;
			}

			if (value is DateTime)
			{
				// Only take the date part into account.
				_builder.AppendLine($"DATEDIFF(day, [{columnName}], {paramName}) = 0");
				_command.AddSqlParameter(paramName, value, sqlType);

				return this;
			}

			_builder.AppendLine($"[{columnName}] = {paramName}");
			_command.AddSqlParameter(paramName, value, sqlType);

			return this;
		}

		/// <summary>
		/// Adds a date range to the where clause.
		/// Since a datetime without a specified time segment will have a value of date 00:00:00.000, if you want to be 
		/// sure you get all the dates in your range, you must either supply the time for your ending date or increase your 
		/// ending date and use '<'.
		/// We use the latter approach.
		/// </summary>
		/// <param name="range"></param>
		/// <param name="columnName"></param>
		/// <param name="sqlType"></param>
		/// <returns></returns>
		public WhereClauseBuilder ForDateRange(
			DateRange range, string columnName, SqlDbType sqlType = SqlDbType.DateTime2)
		{
			if (range == null)
				return this;

			HandleFirstParameter();

			if (range.StartDate.HasValue)
			{
				string paramName = $"@{columnName}Start";
				_builder.Append($"[{columnName}] >= {paramName}");
				_command.AddSqlParameter(paramName, range.StartDate.Value.Date, sqlType);
			}

			if (range.EndDate.HasValue)
			{
				if (range.StartDate.HasValue)
					_builder.Append(" AND ");

				string paramName = $"@{columnName}End";
				_builder.Append($"[{columnName}] < {paramName}");
				_command.AddSqlParameter(paramName, range.EndDate.Value.Date.AddDays(1), sqlType);
			}

			_builder.AppendLine();

			return this;
		}

		public override string ToString()
		{
			if (!_isFirstParameter)
				_builder.AppendLine(")");

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