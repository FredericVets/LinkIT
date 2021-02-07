using LinkIT.Data.DTO;
using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using System.Collections.Generic;

namespace LinkIT.Data.Repositories
{
	public interface IAssetRepository : IRepository<AssetDto, AssetQuery>
	{
		PagedResult<AssetDto> ForOwners(PageInfo pageInfo, IEnumerable<string> owners);
	}
}