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
			OrderByField = "+Id";
		}

		[Range(0, int.MaxValue)]
		public int PageNumber { get; set; }

		[Range(0, int.MaxValue)]
		public int RowsPerPage { get; set; }

		[MaxLength(30)]
		[RegularExpression(OrderBy.REGEX_PATTERN)]
		public string OrderByField { get; set; }

		public OrderBy GetOrderBy() => OrderBy.Parse(OrderByField);
	}
}