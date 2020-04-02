using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace LinkIT.Data.Builders
{
	public class SelectCommandBuilder
	{
		private readonly SqlConnection _con;
		private readonly SqlTransaction _tx;
		private readonly bool _hasSoftDelete;
		private readonly SqlCommand _cmd;

		private string _selectStatement;
		private string _whereClause;
		private PageInfo _pageInfo;

		public SelectCommandBuilder(SqlConnection con, SqlTransaction tx, bool hasSoftDelete)
		{
			_con = con ?? throw new ArgumentNullException(nameof(con));
			_tx = tx ?? throw new ArgumentNullException(nameof(tx));
			_hasSoftDelete = hasSoftDelete;

			_cmd = new SqlCommand { Connection = _con, Transaction = _tx };
		}

		private void Validate()
		{
			if (string.IsNullOrWhiteSpace(_selectStatement))
				throw new InvalidOperationException("Select statement is required.");
		}

		private void AddPaging(StringBuilder output)
		{
			output.AppendLine($"ORDER BY [{_pageInfo.OrderBy.Name}] {_pageInfo.OrderBy.Order.ForSql()}");
			output.AppendLine("OFFSET ((@PageNumber - 1) * @RowsPerPage) ROWS");
			output.AppendLine("FETCH NEXT @RowsPerPage ROWS ONLY");

			_cmd.Parameters.Add("@PageNumber", SqlDbType.Int).Value = _pageInfo.PageNumber;
			_cmd.Parameters.Add("@RowsPerPage", SqlDbType.Int).Value = _pageInfo.RowsPerPage;
		}

		public SelectCommandBuilder ForSelect(string select)
		{
			_selectStatement = select;

			return this;
		}

		public SelectCommandBuilder ForWhere<TQuery>(
			TQuery query,
			Action<TQuery, WhereClauseBuilder> addParamsToBuilderAction) where TQuery : Query
		{
			if (!string.IsNullOrWhiteSpace(_whereClause))
				throw new InvalidOperationException("Where clause is already specified.");

			if (query == null)
			{
				_whereClause = new WhereClauseBuilder(_cmd, _hasSoftDelete).ToString();

				return this;
			}

			var builder = new WhereClauseBuilder(_cmd, query.LogicalOperator, _hasSoftDelete);
			addParamsToBuilderAction(query, builder);
			_whereClause = builder.ToString();

			return this;
		}

		public SelectCommandBuilder ForWhereIn<T>(string columnName, IEnumerable<T> values, SqlDbType sqlType)
		{
			if (!string.IsNullOrWhiteSpace(_whereClause))
				throw new InvalidOperationException("Where clause is already specified.");

			var builder = new WhereInClauseBuilder(columnName, _cmd, _hasSoftDelete);
			builder.ForParameters(values, sqlType);
			_whereClause = builder.ToString();

			return this;
		}

		public SelectCommandBuilder ForPaging(PageInfo pageInfo)
		{
			_pageInfo = pageInfo;

			return this;
		}

		public SqlCommand Build()
		{
			Validate();

			var sb = new StringBuilder();
			sb.AppendLine(_selectStatement);

			if (!string.IsNullOrWhiteSpace(_whereClause))
				sb.Append(_whereClause);

			if (_pageInfo != null)
				AddPaging(sb);

			_cmd.CommandText = sb.ToString();

			return _cmd;
		}
	}
}