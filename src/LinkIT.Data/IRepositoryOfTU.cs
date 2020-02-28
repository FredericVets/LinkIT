using LinkIT.Data.DTO;
using System.Collections.Generic;

namespace LinkIT.Data
{
	public interface IRepository<T, U>
		where T : Dto
		where U : Query
	{
		bool Exists(long id);

		T Get(long id);

		IEnumerable<T> Get(IEnumerable<long> ids);

		IEnumerable<T> Query(U query = null);

		PagedResult<T> PagedQuery(Paging paging, U query = null);

		long Insert(T item);

		void Update(T item);

		void Update(IEnumerable<T> data);

		void Delete(long id);
	}
}