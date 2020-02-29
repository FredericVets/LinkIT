using LinkIT.Data.DTO;
using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using System.Collections.Generic;

namespace LinkIT.Data.Repositories
{
	public interface IRepository<T, U>
		where T : Dto
		where U : Query
	{
		bool Exists(long id);

		T GetById(long id);

		IEnumerable<T> GetById(IEnumerable<long> ids);

		IEnumerable<T> Query(U query = null);

		PagedResult<T> PagedQuery(PageInfo paging, U query = null);

		long Insert(T item);

		void Update(T item);

		void Update(IEnumerable<T> data);

		void Delete(long id);
	}
}