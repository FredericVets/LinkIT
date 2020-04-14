using LinkIT.Data.DTO;
using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using LinkIT.Web.Filters.Api;
using LinkIT.Web.Models.Api;
using LinkIT.Web.Models.Api.Filters;
using LinkIT.Web.Models.Api.Paging;
using log4net;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace LinkIT.Web.Controllers.Api
{
	public class SpecialOwnersController : ApiController
	{
		private readonly IRepository<SpecialOwnerDto, SpecialOwnerQuery> _repo;
		private readonly ILog _log;

		public SpecialOwnersController(IRepository<SpecialOwnerDto, SpecialOwnerQuery> repo)
		{
			_repo = repo;
			_log = LogManager.GetLogger(GetType());
		}

		private static SpecialOwnerReadModel MapToModel(SpecialOwnerDto input) =>
			new SpecialOwnerReadModel
			{
				Id = input.Id.Value,
				CreationDate = input.CreationDate.Value,
				CreatedBy = input.CreatedBy,
				ModificationDate = input.ModificationDate.Value,
				ModifiedBy = input.ModifiedBy,
				Name = input.Name,
				Remark = input.Remark
			};

		private static SpecialOwnerDto MapToDto(SpecialOwnerWriteModel input, long? id = null) =>
			new SpecialOwnerDto
			{
				Id = id,
				CreatedBy = input.CreatedBy,
				ModifiedBy = input.ModifiedBy,
				Name = input.Name,
				Remark = input.Remark
			};

		private static SpecialOwnerQuery MapToQuery(SpecialOwnerFilterModel filter) =>
			new SpecialOwnerQuery
			{
				CreatedBy = filter.CreatedBy,
				ModifiedBy = filter.ModifiedBy,
				Name = filter.Name,
				Remark = filter.Remark
			};

		private IHttpActionResult CreateActionResultFor(PagedResult<SpecialOwnerDto> pagedResult)
		{
			if (pagedResult.IsEmpty())
				return StatusCode(HttpStatusCode.NoContent);

			var models = pagedResult.Result.Select(MapToModel).ToList();
			var result = new PagedResultModel<SpecialOwnerReadModel>(
				models,
				MappingHelper.MapToModel(pagedResult.PageInfo),
				pagedResult.TotalCount);

			return Ok(result);
		}

		[Route("api/special-owners/{id:long:min(1)}", Name = "GetSpecialOwnerById")]
		[ShibbolethAuthorize(Roles = Constants.Roles.READ)]
		public IHttpActionResult Get(long id)
		{
			if (!_repo.Exists(id))
				return NotFound();

			var dto = _repo.GetById(id);
			var readModel = MapToModel(dto);

			return Ok(readModel);
		}

		[Route("api/special-owners")]
		[ShibbolethAuthorize(Roles = Constants.Roles.READ)]
		public IHttpActionResult Get(
			[FromUri]SpecialOwnerFilterModel filter,
			[FromUri]PageInfoModel pageInfo)
		{
			filter = filter ?? new SpecialOwnerFilterModel();
			pageInfo = pageInfo ?? new PageInfoModel();
			PagedResult<SpecialOwnerDto> pagedResult;

			var paging = MappingHelper.MapToPageInfo(pageInfo);
			if (!paging.OrderBy.IsValidFor(_repo.Columns))
				return BadRequest($"Unknown field : {paging.OrderBy.Name}.");

			if (filter.IsEmpty())
			{
				pagedResult = _repo.PagedQuery(paging);

				return CreateActionResultFor(pagedResult);
			}

			// Apply filter.
			var query = MapToQuery(filter);
			pagedResult = _repo.PagedQuery(paging, query);

			return CreateActionResultFor(pagedResult);
		}

		[Route("api/special-owners")]
		[ShibbolethAuthorize(Roles = Constants.Roles.CREATE)]
		public IHttpActionResult Post(SpecialOwnerWriteModel model)
		{
			if (model == null)
				return BadRequest(Constants.MISSING_MESSAGE_BODY);
			if (string.IsNullOrWhiteSpace(model.CreatedBy))
				return BadRequest("CreatedBy is required.");

			var dto = MapToDto(model);
			long id = _repo.Insert(dto);

			// Refetch the data.
			dto = _repo.GetById(id);
			var readModel = MapToModel(dto);

			return CreatedAtRoute("GetSpecialOwnerById", new { id = readModel.Id }, readModel);
		}

		// Fully updates the product.
		[Route("api/special-owners/{id:long:min(1)}")]
		[ShibbolethAuthorize(Roles = Constants.Roles.MODIFY)]
		public IHttpActionResult Put(long id, SpecialOwnerWriteModel model)
		{
			if (model == null)
				return BadRequest(Constants.MISSING_MESSAGE_BODY);
			if (string.IsNullOrWhiteSpace(model.ModifiedBy))
				return BadRequest("ModifiedBy is required.");

			if (!_repo.Exists(id))
				return NotFound();

			var dto = MapToDto(model, id);

			_repo.Update(dto);

			// Refetch the data.
			dto = _repo.GetById(id);
			var readModel = MapToModel(dto);

			return Ok(readModel);
		}

		[Route("api/special-owners/{id:long:min(1)}")]
		[ShibbolethAuthorize(Roles = Constants.Roles.DELETE)]
		public IHttpActionResult Delete(long id)
		{
			if (!_repo.Exists(id))
				return NotFound();

			_repo.Delete(id);

			return StatusCode(HttpStatusCode.NoContent);
		}
	}
}