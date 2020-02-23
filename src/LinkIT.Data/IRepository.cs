using LinkIT.Data.DTO;
using System.Collections.Generic;

namespace LinkIT.Data
{
	interface IRepository<T, U> 
		where T: Dto 
		where U : Query
	{
		IEnumerable<T> Get();

		T Get(long id);

		IEnumerable<T> Query(U query, WhereCondition condition);

		long Insert(T input);

		void Update(T input);

		void Delete(long id);
	}
}