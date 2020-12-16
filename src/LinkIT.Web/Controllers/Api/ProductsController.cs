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
using System.Linq;
using System.Net;
using System.Web.Http;

namespace LinkIT.Web.Controllers.Api
{
	public class ProductsController : ApiController
	{
		private readonly IRepository<ProductDto, ProductQuery> _repo;
		private readonly IJsonWebTokenWrapper _jwt;
		private readonly ILog _log;

		public ProductsController(IRepository<ProductDto, ProductQuery> repo, IJsonWebTokenWrapper jwt)
		{
			_repo = repo;
			_jwt = jwt;
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
				Type = input.Type,
				Group = input.Group
			};

		private static ProductQuery MapToQuery(ProductFilterModel filter) =>
			new ProductQuery
			{
				CreatedBy = filter.CreatedBy,
				ModifiedBy = filter.ModifiedBy,
				Brand = filter.Brand,
				Type = filter.Type,
				Group = filter.Group
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

		private bool AlreadyExists(ProductWriteModel model)
		{
			// All 3 properties are required.
			var query = new ProductQuery
			{
				Brand = model.Brand,
				Type = model.Type,
				Group = model.Group
			};

			return _repo.Query(query).Any();
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
				Type = input.Type,
				Group = input.Group
			};

		[Route("api/products/{id:long:min(1)}", Name = "GetProductById")]
		[JwtAuthorize(Roles = Constants.Roles.READ)]
		public IHttpActionResult GetProductById(long id)
		{
			if (!_repo.Exists(id))
				return NotFound();

			var dto = _repo.GetById(id);
			var readModel = MapToModel(dto);

			return Ok(readModel);
		}

		[Route("api/products")]
		[JwtAuthorize(Roles = Constants.Roles.READ)]
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
		[JwtAuthorize(Roles = Constants.Roles.CREATE)]
		public IHttpActionResult Post(ProductWriteModel model)
		{
			if (model == null)
				return BadRequest(Constants.MISSING_MESSAGE_BODY);

			if (AlreadyExists(model))
				return BadRequest("Product with the specified Brand, Type and Group already exists.");

			var dto = MapToDto(model, createdBy: _jwt.UserId);
			long id = _repo.Insert(dto);

			// Refetch the data.
			dto = _repo.GetById(id);
			var readModel = MapToModel(dto);

			return CreatedAtRoute(nameof(GetProductById), new { id = readModel.Id }, readModel);
		}

		// Fully updates the product.
		[Route("api/products/{id:long:min(1)}")]
		[JwtAuthorize(Roles = Constants.Roles.MODIFY)]
		public IHttpActionResult Put(long id, ProductWriteModel model)
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

		// Delete is not supported for products.
	}
}