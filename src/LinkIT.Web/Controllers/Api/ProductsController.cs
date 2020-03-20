using LinkIT.Data.DTO;
using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using LinkIT.Web.Infrastructure.Api;
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
		private readonly ILog _log;

		public ProductsController()
		{
			_repo = new ProductRepository(ConnectionString.Get());
			_log = LogManager.GetLogger(GetType());
		}

		private static ProductDto MapToDto(ProductWriteModel input, long? id = null)
		{
			return new ProductDto
			{
				Id = id,
				CreatedBy = input.CreatedBy,
				ModifiedBy = input.ModifiedBy,
				Brand = input.Brand,
				Type = input.Type
			};
		}

		private static ProductQuery MapToQuery(ProductFilterModel filter)
		{
			return new ProductQuery
			{
				CreatedBy = filter.CreatedBy,
				ModifiedBy = filter.ModifiedBy,
				Brand = filter.Brand,
				Type = filter.Type
			};
		}

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

		public static ProductReadModel MapToModel(ProductDto input)
		{
			return new ProductReadModel
			{
				Id = input.Id.Value,
				CreationDate = input.CreationDate.Value,
				CreatedBy = input.CreatedBy,
				ModificationDate = input.ModificationDate.Value,
				ModifiedBy = input.ModifiedBy,
				Brand = input.Brand,
				Type = input.Type
			};
		}

		[Route("api/products/{id:long:min(1)}", Name = "GetProductById")]
		public IHttpActionResult Get(long id)
		{
			if (!_repo.Exists(id))
				return NotFound();

			var dto = _repo.GetById(id);
			var readModel = MapToModel(dto);

			return Ok(readModel);
		}

		[Route("api/products")]
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
		public IHttpActionResult Post(ProductWriteModel model)
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

			return CreatedAtRoute("GetProductById", new { id = readModel.Id }, readModel);
		}

		// Fully updates the product.
		[Route("api/products/{id:long:min(1)}")]
		public IHttpActionResult Put(long id, ProductWriteModel model)
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

		// Delete is not supported for products.
	}
}