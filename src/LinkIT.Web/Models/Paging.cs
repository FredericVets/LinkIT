namespace LinkIT.Web.Models
{
	public class Paging
	{
		public Paging()
		{
			PageNumber = 1;
			RowsPerPage = 50;
			OrderBy = "Id";
		}

		public int PageNumber { get; set; }

		public int RowsPerPage { get; set; }

		public string OrderBy { get; set; }

		public int TotalResult { get; set; }
	}
}