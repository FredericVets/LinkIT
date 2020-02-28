namespace LinkIT.Web.Models.Api
{
	public class PagingModel
	{
		public PagingModel()
		{
			// The defaults.
			PageNumber = 1;
			RowsPerPage = 50;
			OrderBy = "Id";
		}

		public int PageNumber { get; set; }

		public int RowsPerPage { get; set; }

		public string OrderBy { get; set; }
	}
}