using LinkIT.Data.DTO;
using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using LinkIT.Web.Models.Api;
using LinkIT.Web.Models.Api.Filters;
using LinkIT.Web.Models.Api.Paging;
using log4net;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace LinkIT.Web.Controllers.Api
{
	public class AssetHistoriesController : ApiController
	{
		private readonly IRepository<AssetHistoryDto, AssetHistoryQuery> _assetHistoryRepo;
		private readonly IAssetRepository _assetRepo;
		private readonly IRepository<ProductDto, ProductQuery> _productRepo;
		private readonly ILog _log;

		public AssetHistoriesController(
			IRepository<AssetHistoryDto, AssetHistoryQuery> assetHistoryRepo,
			IAssetRepository assetRepo,
			 IRepository<ProductDto, ProductQuery> productRepo)
		{
			_assetHistoryRepo = assetHistoryRepo;
			_productRepo = productRepo;
			_assetRepo = assetRepo;

			_log = LogManager.GetLogger(GetType());
		}

		private static AssetHistoryReadModel MapToModel(AssetHistoryDto input)
		{
			var model = new AssetHistoryReadModel();
			MappingHelper.PopulateReadModel(input, model);

			// Map the extra properties belonging only to the AssetHistoryReadModel.
			model.AssetId = input.AssetId;
			model.ProductId = input.Product.Id.Value;

			return model;
		}

		private static AssetHistoryQuery MapToQuery(AssetHistoryFilterModel filter) =>
			new AssetHistoryQuery
			{
				AssetId = filter.AssetId,
				Tag = filter.Tag
			};

		private IHttpActionResult CreateActionResultFor(PagedResult<AssetHistoryDto> pagedResult)
		{
			if (pagedResult.IsEmpty())
				return StatusCode(HttpStatusCode.NoContent);

			var models = pagedResult.Result.Select(MapToModel).ToList();
			var result = new PagedResultModel<AssetHistoryReadModel>(
				models,
				MappingHelper.MapToModel(pagedResult.PageInfo),
				pagedResult.TotalCount);

			return Ok(result);
		}

		[Route("api/asset-histories/{id:long:min(1)}", Name = "GetAssetHistoryById")]
		public IHttpActionResult Get(long id)
		{
			if (!_assetHistoryRepo.Exists(id))
				return NotFound();

			var dto = _assetHistoryRepo.GetById(id);
			var readModel = MapToModel(dto);

			return Ok(readModel);
		}

		[Route("api/asset-histories")]
		public IHttpActionResult Get(
			[FromUri]AssetHistoryFilterModel filter,
			[FromUri]PageInfoModel pageInfo)
		{
			filter = filter ?? new AssetHistoryFilterModel();
			pageInfo = pageInfo ?? new PageInfoModel();
			PagedResult<AssetHistoryDto> pagedResult;

			var paging = MappingHelper.MapToPageInfo(pageInfo);
			if (!paging.OrderBy.IsValidFor(_assetHistoryRepo.Columns))
				return BadRequest($"Unknown field : {paging.OrderBy.Name}.");

			if (filter.IsEmpty())
			{
				pagedResult = _assetHistoryRepo.PagedQuery(paging);

				return CreateActionResultFor(pagedResult);
			}

			// Apply filter.
			var query = MapToQuery(filter);
			pagedResult = _assetHistoryRepo.PagedQuery(paging, query);

			return CreateActionResultFor(pagedResult);
		}

		[Route("api/asset-histories/{id:long:min(1)}/asset")]
		public IHttpActionResult GetAssetFor(long id)
		{
			if (!_assetHistoryRepo.Exists(id))
				return NotFound();

			var historyDto = _assetHistoryRepo.GetById(id);
			var assetDto = _assetRepo.GetById(historyDto.AssetId);

			var readModel = AssetsController.MapToModel(assetDto);

			return Ok(readModel);
		}

		[Route("api/asset-histories/{id:long:min(1)}/product")]
		public IHttpActionResult GetProductFor(long id)
		{
			if (!_assetHistoryRepo.Exists(id))
				return NotFound();

			var historyDto = _assetHistoryRepo.GetById(id);
			var productDto = _productRepo.GetById(historyDto.Product.Id.Value);

			var readModel = ProductsController.MapToModel(productDto);

			return Ok(readModel);
		}
	}
}