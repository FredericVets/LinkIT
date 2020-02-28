using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIT.Web.Models.Api
{
	public class PagedResultModel<T> where T : class
	{
		public PagedResultModel(
			IEnumerable<T> result,
			PagingModel paging,
			long totalCount)
		{
			Result = result;
			Paging = paging;
			TotalCount = totalCount;
		}

		public IEnumerable<T> Result { get; }

		public PagingModel Paging { get; }

		public long TotalCount { get; }
	}
}