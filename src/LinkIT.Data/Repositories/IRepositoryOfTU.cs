using LinkIT.Data.DTO;
using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using System.Collections.Generic;

namespace LinkIT.Data.Repositories
{
	public interface IRepository<TDto, TQuery>
		where TDto : Dto
		where TQuery : Query
	{
		IEnumerable<string> Columns { get; }

		bool Exists(long id);

		bool Exists(IEnumerable<long> ids);

		/// <summary>
		/// Returns the Dto for that id. If not found, throws an Exception.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		TDto GetById(long id);

		/// <summary>
		/// Returns the Dto's for those ids. If one isn't found, throws an Exception.
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		IEnumerable<TDto> GetById(IEnumerable<long> ids);

		/// <summary>
		/// Returns all the Dto's that match the query.
		/// If no query is specified, it returns all the Dto's.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		IEnumerable<TDto> Query(TQuery query = null);

		/// <summary>
		/// Returns all the Dto's that match the query.
		/// If no query is specified, it returns all the Dto's.
		/// The Dto's will be returned in a paging way as specified in the PageInfo instance.
		/// </summary>
		/// <param name="paging"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		PagedResult<TDto> PagedQuery(PageInfo paging, TQuery query = null);

		long Insert(TDto item);

		/// <summary>
		/// This is a full-update. So all required fields should be supplied.
		/// </summary>
		/// <param name="item"></param>
		void Update(TDto item);

		/// <summary>
		/// This is a full-update. So all required fields should be supplied.
		/// </summary>
		/// <param name="items"></param>
		void Update(IEnumerable<TDto> items);

		void Delete(long id);
	}
}