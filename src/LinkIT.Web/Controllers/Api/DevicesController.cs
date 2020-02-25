using LinkIT.Data.DTO;
using LinkIT.Data.Repositories;
using LinkIT.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Web.Http;

namespace LinkIT.Web.Controllers.Api
{
	// See docs at https://www.tutorialsteacher.com/webapi/web-api-tutorials
	public class DevicesController : ApiController
	{
		private DeviceRepository _repo;

		public DevicesController()
		{
			_repo = new DeviceRepository(WebConfigurationManager.ConnectionStrings["LinkITConnectionString"].ConnectionString);
		}

		// GET api/device
		public IEnumerable<DeviceModel> Get()
		{
			// Repository returns "DeviceDto" instances. Map them to "DeviceModel" instances.
			return _repo.Query().Select(x => new DeviceModel
			{
				Id = x.Id,
				Brand = x.Brand,
				Type = x.Type,
				Owner = x.Owner,
				Tag = x.Tag
			});
		}

		// example : GET api/values/5
		public DeviceModel Get(long id)
		{
			var dto = _repo.Get(id);

			return new DeviceModel
			{
				Id = dto.Id,
				Brand = dto.Brand,
				Type = dto.Type,
				Owner = dto.Owner,
				Tag = dto.Tag
			};
		}

		public DeviceModel Post(DeviceModel model)
		{
			var dto = new DeviceDto
			{
				Brand = model.Brand,
				Type = model.Type,
				Owner = model.Owner,
				Tag = model.Tag
			};

			model.Id = _repo.Insert(dto);

			return model;
		}

		// PUT api/values/5
		// Fully updates the Device.
		public void Put(long id, DeviceModel model)
		{
			var dto = new DeviceDto
			{
				Id = id,
				Brand = model.Brand,
				Type = model.Type,
				Owner = model.Owner,
				Tag = model.Tag
			};

			_repo.Update(dto);
		}

		// DELETE api/values/5
		// You can also use the void return type.
		// Then the client will receive status code 204 as result of a DELETE request.
		public void Delete(long id)
		{
			_repo.Delete(id);
		}

		// Partial update of the Device.
		public void Patch(DeviceModel value)
		{
		}
	}
}