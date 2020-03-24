using LinkIT.Data.Paging;
using LinkIT.Web.Models.Api.Paging;

namespace LinkIT.Web.Controllers.Api
{
	public static class MappingHelper
	{
		public static PageInfo MapToPageInfo(PageInfoModel input) =>
			new PageInfo(
				input.PageNumber,
				input.RowsPerPage,
				input.OrderBy);

		public static PageInfoModel MapToModel(PageInfo input) =>
			new PageInfoModel
			{
				PageNumber = input.PageNumber,
				RowsPerPage = input.RowsPerPage,
				OrderBy = input.OrderBy
			};
	}
}