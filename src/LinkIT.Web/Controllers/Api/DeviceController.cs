using LinkIT.Data.Repositories;
using LinkIT.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Web.Http;

namespace LinkIT.Web.Controllers.Api
{
	// See docs at https://www.tutorialsteacher.com/webapi/web-api-tutorials
	public class DeviceController : ApiController
	{
		private DeviceRepository _repo;

		public DeviceController()
		{
			_repo = new DeviceRepository(WebConfigurationManager.ConnectionStrings["LinkITConnectionString"].ConnectionString);
		}

		// GET api/device
		public IEnumerable<DeviceModel> Get()
		{
			return new DeviceModel[]
			{
				new DeviceModel
				{
					Id = 1,
					Tag = "CRD-L-07140",
					Owner = "u0122713",
					Brand = "Dell",
					Type = "Latitude 7390"
				},
				new DeviceModel
				{
					Id = 2,
					Tag = "CRD-L-07654",
					Owner = "u0122713",
					Brand = "HP",
					Type = "EliteBook 750 G6"
				},
			};

			// Repository returns "DeviceDto" instances. Map them to "DeviceModel" instances.
			return _repo.Query().Select(x => new DeviceModel
			{
				Id = x.Id,
				Tag = x.Tag,
				Owner = x.Owner,
				Brand = x.Brand,
				Type = x.Type
			});
		}

		// example : GET api/values/5
		public IHttpActionResult Get(long id)
		{
			if (id == -1)
				return NotFound();

			var device = new DeviceModel
			{
				Id = id,
				Tag = "CRD-L-07140",
				Owner = "u0122713",
				Brand = "Dell",
				Type = "Latitude 7390"
			};

			return Ok(device);
		}

		// POST api/values
		// 2 following attributes are the default behaviour.
		// Create the new Device.
		[HttpPost]
		public void Post(DeviceModel value)
		{
		}

		// PUT api/values/5
		// Fully updates the Device.
		public IHttpActionResult Put(DeviceModel value)
		{
			if (!value.Id.HasValue)
				return NotFound();

			return Ok();
		}

		// DELETE api/values/5
		// You can also use the void return type.
		// Then the client will receive status code 204 as result of a DELETE request.
		public void Delete(long id)
		{
		}

		// Partial update of the Device.
		public void Patch(DeviceModel value)
		{
		}
	}
}