namespace LinkIT.Data.Paging
{
	public class PageInfo
	{
		public PageInfo(
			int pageNumber,
			int rowsPerPage,
			OrderBy orderBy)
		{
			PageNumber = pageNumber;
			RowsPerPage = rowsPerPage;
			OrderBy = orderBy;
		}

		public int PageNumber { get; }

		public int RowsPerPage { get; }

		public OrderBy OrderBy { get; }
	}
}