using LinkIT.Data.DTO;
using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using LinkIT.Web.Filters.Api;
using LinkIT.Web.Infrastructure.Auth;
using LinkIT.Web.Models.Api;
using LinkIT.Web.Models.Api.Filters;
using LinkIT.Web.Models.Api.Paging;
using log4net;
using Swashbuckle.Swagger.Annotations;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace LinkIT.Web.Controllers.Api
{
	public class SpecialOwnersController : ApiController
	{
		private readonly IRepository<SpecialOwnerDto, SpecialOwnerQuery> _repo;
		private readonly IJsonWebTokenWrapper _jwt;
		private readonly ILog _log;

		public SpecialOwnersController(
			IRepository<SpecialOwnerDto, SpecialOwnerQuery> repo,
			IJsonWebTokenWrapper jwt)
		{
			_repo = repo;
			_jwt = jwt;
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

		private SpecialOwnerDto MapToDto(
			SpecialOwnerWriteModel input, long? id = null, string createdBy = null, string modifiedBy = null) =>
			new SpecialOwnerDto
			{
				Id = id,
				CreatedBy = createdBy,
				ModifiedBy = modifiedBy,
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

		/// <summary>
		/// Gets the special-owners that match the filter criteria in a paging fashion.
		/// Filter criteria are optional.
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="pageInfo"></param>
		/// <returns></returns>
		[Route("api/special-owners")]
		[JwtAuthorize(Roles = Constants.Roles.READ)]
		[SwaggerResponse(HttpStatusCode.OK,
			Type = typeof(PagedResultModel<SpecialOwnerReadModel>),
			Description = Consts.SWAGGER_PAGING_RESPONSE_DESCRIPTION)]
		[SwaggerResponse(HttpStatusCode.NoContent)]
		[SwaggerResponse(HttpStatusCode.BadRequest)]
		[SwaggerResponse(HttpStatusCode.Unauthorized)]
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

		/// <summary>
		/// Gets the special-owner with the specified id.
		/// </summary>
		/// <param name="id">The id of the special-owner that is to be retrieved.</param>
		/// <returns></returns>
		[Route("api/special-owners/{id:long:min(1)}", Name = "GetSpecialOwnerById")]
		[JwtAuthorize(Roles = Constants.Roles.READ)]
		[SwaggerResponse(HttpStatusCode.OK, Type = typeof(SpecialOwnerReadModel))]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[SwaggerResponse(HttpStatusCode.Unauthorized)]
		public IHttpActionResult GetSpecialOwnerById(long id)
		{
			if (!_repo.Exists(id))
				return NotFound();

			var dto = _repo.GetById(id);
			var readModel = MapToModel(dto);

			return Ok(readModel);
		}

		/// <summary>
		/// Creates a new special-owner.
		/// </summary>
		/// <param name="model">The new special-owner.</param>
		/// <returns></returns>
		[Route("api/special-owners")]
		[JwtAuthorize(Roles = Constants.Roles.CREATE)]
		[SwaggerResponse(HttpStatusCode.Created, Type = typeof(SpecialOwnerReadModel))]
		[SwaggerResponse(HttpStatusCode.BadRequest)]
		[SwaggerResponse(HttpStatusCode.Unauthorized)]
		public IHttpActionResult Post(SpecialOwnerWriteModel model)
		{
			if (model == null)
				return BadRequest(Constants.MISSING_MESSAGE_BODY);

			var dto = MapToDto(model, createdBy: _jwt.UserId);
			long id = _repo.Insert(dto);

			// Refetch the data.
			dto = _repo.GetById(id);
			var readModel = MapToModel(dto);

			return CreatedAtRoute(nameof(GetSpecialOwnerById), new { id = readModel.Id }, readModel);
		}

		/// <summary>
		/// Fully updates the special-owner with the specified id.
		/// </summary>
		/// <param name="id">The id of the special-owner that is to be updated.</param>
		/// <param name="model">The updated special-owner.</param>
		/// <returns></returns>
		[Route("api/special-owners/{id:long:min(1)}")]
		[JwtAuthorize(Roles = Constants.Roles.MODIFY)]
		[SwaggerResponse(HttpStatusCode.Created, Type = typeof(SpecialOwnerReadModel))]
		[SwaggerResponse(HttpStatusCode.BadRequest)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[SwaggerResponse(HttpStatusCode.Unauthorized)]
		public IHttpActionResult Put(long id, SpecialOwnerWriteModel model)
		{
			if (model == null)
				return BadRequest(Constants.MISSING_MESSAGE_BODY);

			if (!_repo.Exists(id))
				return NotFound();

			var dto = MapToDto(model, id, modifiedBy: _jwt.UserId);

			_repo.Update(dto);

			// Refetch the data.
			dto = _repo.GetById(id);
			var readModel = MapToModel(dto);

			return Ok(readModel);
		}

		/// <summary>
		/// Deletes the special-owner with the specified id.
		/// This is a hard-delete : the record will be physically deleted.
		/// </summary>
		/// <param name="id">The id of the special-owner that is to be deleted.</param>
		/// <returns></returns>
		[Route("api/special-owners/{id:long:min(1)}")]
		[JwtAuthorize(Roles = Constants.Roles.DELETE)]
		[SwaggerResponse(HttpStatusCode.NoContent)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[SwaggerResponse(HttpStatusCode.Unauthorized)]
		public IHttpActionResult Delete(long id)
		{
			if (!_repo.Exists(id))
				return NotFound();

			_repo.Delete(id);

			return StatusCode(HttpStatusCode.NoContent);
		}
	}
}