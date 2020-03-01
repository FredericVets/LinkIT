using LinkIT.Data.DTO;
using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using LinkIT.Web.Models.Api;
using LinkIT.Web.Models.Api.Filters;
using LinkIT.Web.Models.Api.Paging;
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
		private const int BULK_PUT_THRESHOLD = 50;

		private readonly IDeviceRepository _repo;

		public DevicesController()
		{
			_repo = new DeviceRepository(WebConfigurationManager.ConnectionStrings["LinkITConnectionString"].ConnectionString);
		}

		private static DeviceQuery MapToQuery(DeviceFilterModel filter)
		{
			return new DeviceQuery
			{
				Brand = filter.Brand,
				Type = filter.Type,
				Owner = filter.Owner,
				Tag = filter.Tag
			};
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

		private static PageInfo MapToPageInfo(PageInfoModel input)
		{
			return new PageInfo(
				input.PageNumber,
				input.RowsPerPage,
				input.GetOrderBy());
		}

		private static PageInfoModel MapToModel(PageInfo input)
		{
			return new PageInfoModel
			{
				PageNumber = input.PageNumber,
				RowsPerPage = input.RowsPerPage,
				OrderBy = input.OrderBy.ToString()
			};
		}

		private static HttpResponseMessage CreateResponseFor(HttpRequestMessage request, PagedResult<DeviceDto> pagedResult)
		{
			if (pagedResult.IsEmpty())
				return request.CreateResponse(HttpStatusCode.NoContent);

			var models = pagedResult.Result.Select(MapToModel).ToList();
			var result = new PagedResultModel<DeviceModel>(
				models,
				MapToModel(pagedResult.PageInfo),
				pagedResult.TotalCount);

			return request.CreateResponse(HttpStatusCode.OK, result);
		}

		// example : GET api/values/5
		public IHttpActionResult Get(long id)
		{
			if (!_repo.Exists(id))
				return NotFound();

			var dto = _repo.GetById(id);
			var model = MapToModel(dto);

			return Ok(model);
		}

		/// <summary>
		/// If no results are found that match the filter, a 204 (No Content) status code is sent.
		/// If multiple fields of the filter are supplied, only results will be returned that match all the fields.
		/// Supports paging. If not supplied, default values will be used.
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="pageinfo"></param>
		/// <returns></returns>
		public HttpResponseMessage Get(
			[FromUri]DeviceFilterModel filter,
			[FromUri]PageInfoModel pageinfo)
		{
			var paging = MapToPageInfo(pageinfo);
			PagedResult<DeviceDto> pagedResult;

			// Repository returns "DeviceDto" instances. Map them to "DeviceModel" instances.
			if (filter.IsEmpty())
			{
				pagedResult = _repo.PagedQuery(paging);

				return CreateResponseFor(Request, pagedResult);
			}

			// Apply filter.
			var query = MapToQuery(filter);
			pagedResult = _repo.PagedQuery(paging, query);

			return CreateResponseFor(Request, pagedResult);
		}

		public IHttpActionResult Post(DeviceModel model)
		{
			if (model.Id.HasValue)
				return BadRequest("Id can not be specified.");

			var dto = MapToDto(model);
			long id = _repo.Insert(dto);

			// Refetch the data.
			dto = _repo.GetById(id);
			model = MapToModel(dto);

			return Created($"api/{model.Id}", model);
		}

		// Fully updates the devices.
		public IHttpActionResult Put(IEnumerable<DeviceModel> models)
		{
			if (models.Count() > BULK_PUT_THRESHOLD)
				return BadRequest($"Maximum {BULK_PUT_THRESHOLD} elements can be modified in one request.");

			foreach (var model in models)
			{
				if (!model.Id.HasValue)
					return BadRequest("No id specified for device.");
			}

			long[] ids = models.Select(x => x.Id.Value).ToArray();

			if (!_repo.Exists(ids))
				return BadRequest("Not all supplied id's exist.");

			var dtos = models.Select(MapToDto);
			_repo.Update(dtos);

			// Refetch the data.
			dtos = _repo.GetById(ids);
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
			dto = _repo.GetById(id);
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