using LinkIT.Data.DTO;
using System.Collections.Generic;
using System.Linq;

namespace LinkIT.Data
{
	public class PagedResult<T> where T : Dto
	{
		public PagedResult(
			IEnumerable<T> result,
			Paging paging,
			long totalCount)
		{
			Result = result;
			Paging = paging;
			TotalCount = totalCount;
		}

		public IEnumerable<T> Result { get; }

		public Paging Paging { get; }

		public long TotalCount { get; }

		public bool IsEmpty() => Result.Count() == 0;
	}
}