using LinkIT.Data.DTO;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using LinkIT.Web.Models;
using LinkIT.Web.Models.Filters;
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

		private static HttpResponseMessage CreateResponseFor(HttpRequestMessage request, ICollection<DeviceDto> dtoData)
		{
			if (dtoData.Count == 0)
				return request.CreateResponse(HttpStatusCode.NoContent);

			var modelResult = dtoData.Select(x => new DeviceModel
			{
				Id = x.Id,
				Brand = x.Brand,
				Type = x.Type,
				Owner = x.Owner,
				Tag = x.Tag
			}).ToList();

			return request.CreateResponse(HttpStatusCode.OK, modelResult);
		}

		/// <summary>
		/// If no results are found that match the filter, a 204 (No Content) status code is sent.
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		public HttpResponseMessage Get([FromUri]DeviceFilter filter = null)
		{
			IList<DeviceDto> dtoResult;

			// TODO : fix if no arguments are supplied.
			// Repository returns "DeviceDto" instances. Map them to "DeviceModel" instances.
			if (filter == null)
			{
				dtoResult = _repo.Query().ToList();

				return CreateResponseFor(Request, dtoResult);
			}

			// Apply filter.
			var query = new DeviceQuery
			{
				Brand = filter.Brand,
				Type = filter.Type,
				Owner = filter.Owner,
				Tag = filter.Tag
			};

			dtoResult = _repo.Query(query).ToList();

			return CreateResponseFor(Request, dtoResult);
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

			_repo.Update(dtos);
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