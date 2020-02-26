using LinkIT.Data.DTO;
using System.Collections.Generic;

namespace LinkIT.Data
{
	interface IRepository<T, U>
		where T : Dto
		where U : Query
	{
		bool Exists(long id);

		T Get(long id);

		IEnumerable<T> Query(U query = null, WhereCondition condition = WhereCondition.AND, Paging paging = null);

		long Insert(T item);

		void Update(T item);

		void Update(IEnumerable<T> data);

		void Delete(long id);
	}
}