using LinkIT.Data.DTO;
using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using LinkIT.Web.Filters.Api;
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

		/// <summary>
		/// Gets the asset-histories that match the filter criteria in a paging fashion.
		/// Filter criteria are optional.
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="pageInfo"></param>
		/// <returns></returns>
		[Route("api/asset-histories")]
		[JwtAuthorize(Roles = Constants.Roles.READ)]
		[SwaggerResponse(HttpStatusCode.OK,
			Type = typeof(PagedResultModel<AssetHistoryReadModel>),
			Description = Consts.SWAGGER_PAGING_RESPONSE_DESCRIPTION)]
		[SwaggerResponse(HttpStatusCode.NoContent)]
		[SwaggerResponse(HttpStatusCode.BadRequest)]
		[SwaggerResponse(HttpStatusCode.Unauthorized)]
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

		/// <summary>
		/// Gets the asset-history with the specified id.
		/// </summary>
		/// <param name="id">The id of the asset-history that is to be retrieved.</param>
		/// <returns></returns>
		[Route("api/asset-histories/{id:long:min(1)}")]
		[JwtAuthorize(Roles = Constants.Roles.READ)]
		[SwaggerResponse(HttpStatusCode.OK, Type = typeof(AssetHistoryReadModel))]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[SwaggerResponse(HttpStatusCode.Unauthorized)]
		public IHttpActionResult GetAssetHistoryById(long id)
		{
			if (!_assetHistoryRepo.Exists(id))
				return NotFound();

			var dto = _assetHistoryRepo.GetById(id);
			var readModel = MapToModel(dto);

			return Ok(readModel);
		}

		/// <summary>
		/// Gets the asset referenced by the asset-history with the specified id.
		/// </summary>
		/// <param name="id">The id of the asset-history that is to be retrieved.</param>
		/// <returns></returns>
		[Route("api/asset-histories/{id:long:min(1)}/asset")]
		[JwtAuthorize(Roles = Constants.Roles.READ)]
		[SwaggerResponse(HttpStatusCode.OK, Type = typeof(AssetReadModel))]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[SwaggerResponse(HttpStatusCode.Unauthorized)]
		public IHttpActionResult GetAssetFor(long id)
		{
			if (!_assetHistoryRepo.Exists(id))
				return NotFound();

			var historyDto = _assetHistoryRepo.GetById(id);
			var assetDto = _assetRepo.GetById(historyDto.AssetId);

			var readModel = AssetsController.MapToModel(assetDto);

			return Ok(readModel);
		}

		/// <summary>
		/// Gets the product referenced by the asset-history with the specified id.
		/// </summary>
		/// <param name="id">The id of the asset-history that is to be retrieved.</param>
		/// <returns></returns>
		[Route("api/asset-histories/{id:long:min(1)}/product")]
		[JwtAuthorize(Roles = Constants.Roles.READ)]
		[SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProductReadModel))]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[SwaggerResponse(HttpStatusCode.Unauthorized)]
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