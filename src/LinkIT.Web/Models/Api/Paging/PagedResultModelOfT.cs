using System.Collections.Generic;

namespace LinkIT.Web.Models.Api.Paging
{
	public class PagedResultModel<T>
	{
		public PagedResultModel(
			IEnumerable<T> result,
			PageInfoModel pageInfo,
			long totalCount)
		{
			Result = result;
			PageInfo = pageInfo;
			TotalCount = totalCount;
		}

		public IEnumerable<T> Result { get; }

		public PageInfoModel PageInfo { get; }

		/// <summary>
		/// The total number of records. This does not take paging into account.
		/// </summary>
		public long TotalCount { get; }
	}
}