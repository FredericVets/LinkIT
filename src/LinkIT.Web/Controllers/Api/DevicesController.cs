using LinkIT.Data.DTO;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using LinkIT.Web.Models.Api;
using LinkIT.Web.Models.Api.Filters;
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
		private IDeviceRepository _repo;

		public DevicesController()
		{
			_repo = new DeviceRepository(WebConfigurationManager.ConnectionStrings["LinkITConnectionString"].ConnectionString);
		}

		private static DeviceModel MapToModel(DeviceDto input)
		{
			return new DeviceModel
			{
				Id = input.Id,
				Brand = input.Brand,
				Type = input.Type,
				Owner = input.Owner,
				Tag = input.Tag
			};
		}

		private static DeviceDto MapToDto(DeviceModel input)
		{
			return new DeviceDto
			{
				Id = input.Id,
				Brand = input.Brand,
				Type = input.Type,
				Owner = input.Owner,
				Tag = input.Tag
			};
		}

		private static HttpResponseMessage CreateResponseFor(HttpRequestMessage request, ICollection<DeviceDto> dtoData)
		{
			if (dtoData.Count == 0)
				return request.CreateResponse(HttpStatusCode.NoContent);

			var modelResult = dtoData.Select(MapToModel).ToList();

			return request.CreateResponse(HttpStatusCode.OK, modelResult);
		}

		/// <summary>
		/// If no results are found that match the filter, a 204 (No Content) status code is sent.
		/// If multiple fields of the filter are supplied, only results will be returned that match all the fields.
		/// Supports paging. If not supplied, default values will be used.
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="paging"></param>
		/// <returns></returns>
		public HttpResponseMessage Get(
			[FromUri]DeviceFilter filter,
			[FromUri]Paging paging)
		{
			IList<DeviceDto> dtoResult;

			// Repository returns "DeviceDto" instances. Map them to "DeviceModel" instances.
			if (filter.IsEmpty())
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
			var model = MapToModel(dto);

			return Ok(model);
		}

		public IHttpActionResult Post(DeviceModel model)
		{
			var dto = MapToDto(model);
			long id = _repo.Insert(dto);

			// Refetch the data.
			dto = _repo.Get(id);
			model = MapToModel(dto);

			return Created($"api/{model.Id}", model);
		}

		// Fully updates the devices.
		public IHttpActionResult Put(IEnumerable<DeviceModel> models)
		{
			if (models.Count() > 50)
				return BadRequest("Maximum 50 elements can be modified in one request.");

			foreach (var model in models)
			{
				if (!model.Id.HasValue)
					return BadRequest("No id specified for device.");

				if (!_repo.Exists(model.Id.Value))
					return BadRequest($"No device found for id : {model.Id.Value}.");
			}

			var dtos = models.Select(MapToDto);
			_repo.Update(dtos);

			// Refetch the data.
			long[] ids = models.Select(x => x.Id.Value).ToArray();
			dtos = _repo.Get(ids);

			models = dtos.Select(MapToModel).ToList();

			return Ok(models);
		}

		// PUT api/values/5
		// Fully updates the device.
		public IHttpActionResult Put(long id, DeviceModel model)
		{
			if (!_repo.Exists(id))
				return NotFound();

			model.Id = id;
			var dto = MapToDto(model);
			_repo.Update(dto);

			// Refetch the data.
			dto = _repo.Get(id);
			model = MapToModel(dto);

			return Ok(model);
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