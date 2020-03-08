using LinkIT.Data.Paging;
using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Models.Api.Paging
{
	public class PageInfoModel
	{
		public PageInfoModel()
		{
			// The defaults.
			PageNumber = 1;
			RowsPerPage = 50;
			OrderBy = new OrderBy("Id", Order.ASCENDING);
		}

		[Range(0, int.MaxValue)]
		public int PageNumber { get; set; }

		[Range(0, int.MaxValue)]
		public int RowsPerPage { get; set; }
		
		// Uses a TypeConverter to convert from a string request parameter to an OrderBy instance.
		public OrderBy OrderBy { get; set; }
	}
}