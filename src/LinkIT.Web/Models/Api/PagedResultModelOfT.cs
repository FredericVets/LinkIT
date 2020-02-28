﻿using System.Collections.Generic;

namespace LinkIT.Web.Models.Api
{
	public class PagedResultModel<T> where T : class
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

		public long TotalCount { get; }
	}
}