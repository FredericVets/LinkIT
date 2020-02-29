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
			OrderBy = "+Id";
		}

		[Range(0, int.MaxValue)]
		public int PageNumber { get; set; }

		[Range(0, int.MaxValue)]
		public int RowsPerPage { get; set; }

		[MaxLength(30)]
		[RegularExpression(
			Data.Paging.OrderBy.REGEX_PATTERN,
			ErrorMessage = "Invalid format. Use '(+/-)fieldname'. '+' means ascending, '-' descending. If omitted, defaults to ascending.")]
		public string OrderBy { get; set; }

		public OrderBy GetOrderBy() => Data.Paging.OrderBy.Parse(OrderBy);
	}
}