using System.Collections.Generic;

namespace LinkIT.Data.Repositories
{
	/// <summary>
	/// The base repository for all Dto's.
	/// </summary>
	public interface IRepository
	{
		bool Exists(long id);

		bool Exists(IEnumerable<long> ids);

		void Delete(long id);
	}
}