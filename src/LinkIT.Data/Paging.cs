using System;

namespace LinkIT.Data
{
	public class Paging
	{
		public Paging(int pageNumber, int rowsPerPage, string orderByColumnName)
		{
			if (string.IsNullOrWhiteSpace(orderByColumnName))
				throw new ArgumentNullException("orderByColumnName");

			PageNumber = pageNumber;
			RowsPerPage = rowsPerPage;
			OrderByColumnName = orderByColumnName;
		}

		public int PageNumber { get; }

		public int RowsPerPage { get; }

		public string OrderByColumnName { get; }
	}
}