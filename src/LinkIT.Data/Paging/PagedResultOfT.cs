using LinkIT.Data.DTO;
using System.Collections.Generic;
using System.Linq;

namespace LinkIT.Data.Paging
{
	public class PagedResult<T> where T : Dto
	{
		public PagedResult(
			IEnumerable<T> result,
			PageInfo pageInfo,
			long totalCount)
		{
			Result = result;
			PageInfo = pageInfo;
			TotalCount = totalCount;
		}

		public IEnumerable<T> Result { get; }

		public PageInfo PageInfo { get; }

		public long TotalCount { get; }

		public bool IsEmpty() => 
			!Result.Any();
	}
}