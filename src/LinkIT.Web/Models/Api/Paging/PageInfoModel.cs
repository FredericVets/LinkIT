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
			RowsPerPage = 25;
			OrderBy = new OrderBy("Id", Order.ASCENDING);
		}

		/// <summary>
		/// Paging : the current pageNumber. Defaults to 1.
		/// </summary>
		[Range(0, int.MaxValue)]
		public int PageNumber { get; set; }

		/// <summary>
		/// Paging : the number of rows that are present on one page. Defaults to 25.
		/// </summary>
		[Range(0, 500)]
		public int RowsPerPage { get; set; }

		// Uses a TypeConverter to convert from a string request parameter to an OrderBy instance.
		/// <summary>
		/// Paging : the name of the property on which ordering is applied.
		/// For ascending ordering, the column name is prefixed with the '+' character.
		/// For descending ordering, the column name is prefixed with the '-' character.
		/// Defaults to '+Id'.
		/// </summary>
		public OrderBy OrderBy { get; set; }
	}
}