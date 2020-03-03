using LinkIT.Data.DTO;
using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using LinkIT.Web.Models.Api;
using LinkIT.Web.Models.Api.Filters;
using LinkIT.Web.Models.Api.Paging;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Configuration;
using System.Web.Http;

namespace LinkIT.Web.Controllers.Api
{
	// See docs at https://www.tutorialsteacher.com/webapi/web-api-tutorials for introduction.
	public class DevicesController : ApiController
	{
		private const string MISSING_MESSAGE_BODY = "No message body present.";
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

		private IHttpActionResult CreateResultFor(PagedResult<DeviceDto> pagedResult)
		{
			if (pagedResult.IsEmpty())
				return StatusCode(HttpStatusCode.NoContent);

			var models = pagedResult.Result.Select(MapToModel).ToList();
			var result = new PagedResultModel<DeviceModel>(
				models,
				MappingHelper.MapToModel(pagedResult.PageInfo),
				pagedResult.TotalCount);

			return Ok(result);
		}

		[Route("api/devices/{id:long:min(1)}", Name = "GetDeviceById")]
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
		/// Uses paging on the resulting collection. If no paging info is supplied, default values will be used.
		/// </summary>
		/// <param name="filterModel"></param>
		/// <param name="pageInfoModel"></param>
		/// <returns>A paged collection of DeviceModel instances that match the filter.</returns>
		[Route("api/devices")]
		public IHttpActionResult Get(
			[FromUri]DeviceFilterModel filterModel,
			[FromUri]PageInfoModel pageInfoModel)
		{
			pageInfoModel = pageInfoModel ?? new PageInfoModel();
			filterModel = filterModel ?? new DeviceFilterModel();
			PagedResult<DeviceDto> pagedResult;

			var paging = MappingHelper.MapToPageInfo(pageInfoModel);
			if (!paging.OrderBy.IsValidFor(DeviceRepository.COLUMNS))
				return BadRequest($"Unknown field : {paging.OrderBy.Name}.");

			if (filterModel.IsEmpty())
			{
				pagedResult = _repo.PagedQuery(paging);

				return CreateResultFor(pagedResult);
			}

			// Apply filter.
			var query = MapToQuery(filterModel);
			pagedResult = _repo.PagedQuery(paging, query);

			return CreateResultFor(pagedResult);
		}

		[Route("api/devices")]
		public IHttpActionResult Post(DeviceModel model)
		{
			if (model == null)
				return BadRequest(MISSING_MESSAGE_BODY);

			if (model.Id.HasValue)
				return BadRequest("Id can not be specified.");

			var dto = MapToDto(model);
			long id = _repo.Insert(dto);

			// Refetch the data.
			dto = _repo.GetById(id);
			model = MapToModel(dto);

			return CreatedAtRoute("GetDeviceById", new { id = model.Id }, model);
		}

		// Fully updates the devices.
		[Route("api/devices")]
		public IHttpActionResult Put(IEnumerable<DeviceModel> models)
		{
			if (models == null)
				return BadRequest(MISSING_MESSAGE_BODY);

			if (models.Count() > BULK_PUT_THRESHOLD)
				return BadRequest($"Maximum {BULK_PUT_THRESHOLD} elements can be updated in one request.");

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

		// Fully updates the device.
		[Route("api/devices/{id:long:min(1)}")]
		public IHttpActionResult Put(long id, DeviceModel model)
		{
			if (model == null)
				return BadRequest(MISSING_MESSAGE_BODY);

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

		[Route("api/devices/{id:long:min(1)}")]
		public IHttpActionResult Delete(long id)
		{
			if (!_repo.Exists(id))
				return NotFound();

			_repo.Delete(id);

			return StatusCode(HttpStatusCode.NoContent);
		}
	}
}