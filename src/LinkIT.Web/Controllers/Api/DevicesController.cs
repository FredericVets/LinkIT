using LinkIT.Data.DTO;
using LinkIT.Data.Repositories;
using LinkIT.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
		public IHttpActionResult Get(long id)
		{
			if (!_repo.Exists(id))
				return NotFound();

			var dto = _repo.Get(id);

			return Ok(dto);
		}

		public IHttpActionResult Post(DeviceModel model)
		{
			var dto = new DeviceDto
			{
				Brand = model.Brand,
				Type = model.Type,
				Owner = model.Owner,
				Tag = model.Tag
			};

			model.Id = _repo.Insert(dto);

			return Created(string.Format("api/{0}", model.Id), model);
		}

		public void Put(IEnumerable<DeviceModel> data)
		{
			var dtos = data.Select(x => new DeviceDto
			{
				Id = x.Id,
				Brand = x.Brand,
				Type = x.Type,
				Owner = x.Owner,
				Tag = x.Tag
			});
		}

		// PUT api/values/5
		// Fully updates the Device.
		public IHttpActionResult Put(long id, DeviceModel model)
		{
			if (!_repo.Exists(id))
				return NotFound();

			var dto = new DeviceDto
			{
				Id = id,
				Brand = model.Brand,
				Type = model.Type,
				Owner = model.Owner,
				Tag = model.Tag
			};

			_repo.Update(dto);

			return Ok(dto);
		}

		public HttpResponseMessage Delete(long id)
		{
			if (!_repo.Exists(id))
				return Request.CreateResponse(HttpStatusCode.NotFound);

			_repo.Delete(id);

			return Request.CreateResponse(HttpStatusCode.NoContent);
		}
	}
}