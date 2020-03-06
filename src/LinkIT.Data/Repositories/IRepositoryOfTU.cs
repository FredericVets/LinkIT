using LinkIT.Data.DTO;
using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using System.Collections.Generic;

namespace LinkIT.Data.Repositories
{
	public interface IRepository<T, U> : IRepository
		where T : Dto
		where U : Query
	{
		/// <summary>
		/// Returns the Dto for that id. If not found, throws an Exception.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		T GetById(long id);

		/// <summary>
		/// Returns the Dto's for those ids. If one isn't found, throws an Exception.
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		IEnumerable<T> GetById(IEnumerable<long> ids);

		/// <summary>
		/// Returns all the Dto's that match the query.
		/// If no query is specified, it returns all the Dto's.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		IEnumerable<T> Query(U query = null);

		/// <summary>
		/// Returns all the Dto's that match the query.
		/// If no query is specified, it returns all the Dto's.
		/// The Dto's will be returned in a paging way as specified in the PageInfo instance.
		/// </summary>
		/// <param name="paging"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		PagedResult<T> PagedQuery(PageInfo paging, U query = null);

		long Insert(T item);

		void Update(T item);

		void Update(IEnumerable<T> data);

	}
}