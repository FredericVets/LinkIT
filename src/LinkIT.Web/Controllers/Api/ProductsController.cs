using LinkIT.Data.DTO;
using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using LinkIT.Web.Filters.Api;
using LinkIT.Web.Infrastructure.Shibboleth;
using LinkIT.Web.Models.Api;
using LinkIT.Web.Models.Api.Filters;
using LinkIT.Web.Models.Api.Paging;
using log4net;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace LinkIT.Web.Controllers.Api
{
	public class ProductsController : ApiController
	{
		private readonly IRepository<ProductDto, ProductQuery> _repo;
		private readonly ShibbolethAttributes _shibbolethAttribs;
		private readonly ILog _log;

		public ProductsController(IRepository<ProductDto, ProductQuery> repo, ShibbolethAttributes shibbolethAttribs)
		{
			_repo = repo;
			_shibbolethAttribs = shibbolethAttribs;
			_log = LogManager.GetLogger(GetType());
		}

		private static ProductDto MapToDto(
			ProductWriteModel input, long? id = null, string createdBy = null, string modifiedBy = null) =>
			new ProductDto
			{
				Id = id,
				CreatedBy = createdBy,
				ModifiedBy = modifiedBy,
				Brand = input.Brand,
				Type = input.Type
			};

		private static ProductQuery MapToQuery(ProductFilterModel filter) =>
			new ProductQuery
			{
				CreatedBy = filter.CreatedBy,
				ModifiedBy = filter.ModifiedBy,
				Brand = filter.Brand,
				Type = filter.Type
			};

		private IHttpActionResult CreateActionResultFor(PagedResult<ProductDto> pagedResult)
		{
			if (pagedResult.IsEmpty())
				return StatusCode(HttpStatusCode.NoContent);

			var models = pagedResult.Result.Select(MapToModel).ToList();
			var result = new PagedResultModel<ProductReadModel>(
				models,
				MappingHelper.MapToModel(pagedResult.PageInfo),
				pagedResult.TotalCount);

			return Ok(result);
		}

		public static ProductReadModel MapToModel(ProductDto input) =>
			new ProductReadModel
			{
				Id = input.Id.Value,
				CreationDate = input.CreationDate.Value,
				CreatedBy = input.CreatedBy,
				ModificationDate = input.ModificationDate.Value,
				ModifiedBy = input.ModifiedBy,
				Brand = input.Brand,
				Type = input.Type
			};

		[Route("api/products/{id:long:min(1)}", Name = "GetProductById")]
		[ShibbolethAuthorize(Roles = Constants.Roles.READ)]
		public IHttpActionResult Get(long id)
		{
			if (!_repo.Exists(id))
				return NotFound();

			var dto = _repo.GetById(id);
			var readModel = MapToModel(dto);

			return Ok(readModel);
		}

		[Route("api/products")]
		[ShibbolethAuthorize(Roles = Constants.Roles.READ)]
		public IHttpActionResult Get(
			[FromUri]ProductFilterModel filter,
			[FromUri]PageInfoModel pageInfo)
		{
			filter = filter ?? new ProductFilterModel();
			pageInfo = pageInfo ?? new PageInfoModel();
			PagedResult<ProductDto> pagedResult;

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

		[Route("api/products")]
		[ShibbolethAuthorize(Roles = Constants.Roles.CREATE)]
		public IHttpActionResult Post(ProductWriteModel model)
		{
			if (model == null)
				return BadRequest(Constants.MISSING_MESSAGE_BODY);

			var dto = MapToDto(model, createdBy: _shibbolethAttribs.GetUid());
			long id = _repo.Insert(dto);

			// Refetch the data.
			dto = _repo.GetById(id);
			var readModel = MapToModel(dto);

			return CreatedAtRoute("GetProductById", new { id = readModel.Id }, readModel);
		}

		// Fully updates the product.
		[Route("api/products/{id:long:min(1)}")]
		[ShibbolethAuthorize(Roles = Constants.Roles.MODIFY)]
		public IHttpActionResult Put(long id, ProductWriteModel model)
		{
			if (model == null)
				return BadRequest(Constants.MISSING_MESSAGE_BODY);

			if (!_repo.Exists(id))
				return NotFound();

			var dto = MapToDto(model, id, modifiedBy: _shibbolethAttribs.GetUid());

			_repo.Update(dto);

			// Refetch the data.
			dto = _repo.GetById(id);
			var readModel = MapToModel(dto);

			return Ok(readModel);
		}

		// Delete is not supported for products.
	}
}