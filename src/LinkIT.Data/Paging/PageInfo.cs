using System;

namespace LinkIT.Data.Paging
{
	public class PageInfo
	{
		public PageInfo(
			int pageNumber,
			int rowsPerPage,
			string orderByColumnName,
			Sorting orderBySorting = Sorting.ASCENDING)
		{
			if (string.IsNullOrWhiteSpace(orderByColumnName))
				throw new ArgumentNullException("orderByColumnName");

			PageNumber = pageNumber;
			RowsPerPage = rowsPerPage;
			OrderByColumnName = orderByColumnName;
			OrderBySorting = orderBySorting;
		}

		public int PageNumber { get; }

		public int RowsPerPage { get; }

		public string OrderByColumnName { get; }

		public Sorting OrderBySorting { get; }
	}
}