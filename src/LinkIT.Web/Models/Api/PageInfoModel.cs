using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Models.Api
{
	public class PageInfoModel
	{
		public PageInfoModel()
		{
			// The defaults.
			PageNumber = 1;
			RowsPerPage = 50;
			OrderBy = "Id";
		}

		[Range(0, int.MaxValue)]
		public int PageNumber { get; set; }

		[Range(0, int.MaxValue)]
		public int RowsPerPage { get; set; }

		[MaxLength(30)]
		public string OrderBy { get; set; }
	}
}