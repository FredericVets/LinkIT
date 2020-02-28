using LinkIT.Data.DTO;
using LinkIT.Data.Queries;

namespace LinkIT.Data.Repositories
{
	public interface IDeviceRepository : IRepository<DeviceDto, DeviceQuery> { }
}