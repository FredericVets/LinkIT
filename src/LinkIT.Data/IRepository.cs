using LinkIT.Data.DTO;
using System.Collections.Generic;

namespace LinkIT.Data
{
	interface IRepository<T, U>
		where T : Dto
		where U : Query
	{
		T Get(long id);

		IEnumerable<T> Query(U query = null, WhereCondition condition = WhereCondition.AND, Paging paging = null);

		long Insert(T input);

		void Update(T input);

		void Delete(long id);
	}
}