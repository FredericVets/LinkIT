using LinkIT.Data.DTO;
using LinkIT.Data.Queries;
using System.Collections.Generic;

namespace LinkIT.Data.Repositories
{
	public interface IAssetRepository : IRepository<AssetDto, AssetQuery>
	{
		IEnumerable<AssetDto> ForOwners(IEnumerable<string> owners);
	}
}