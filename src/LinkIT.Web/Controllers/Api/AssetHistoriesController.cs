using LinkIT.Data.DTO;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using LinkIT.Web.Infrastructure.Api;
using LinkIT.Web.Models.Api;
using LinkIT.Web.Models.Api.Filters;
using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace LinkIT.Web.Controllers.Api
{
	public class AssetHistoriesController : ApiController
	{
		private readonly IRepository<AssetHistoryDto, AssetHistoryQuery> _assetHistoryRepo;
		private readonly IAssetRepository _assetRepo;
		private readonly ILog _log;

		public AssetHistoriesController()
		{
			_assetHistoryRepo = new AssetHistoryRepository(ConnectionString.Get());
			
			var productRepo = new ProductRepository(ConnectionString.Get());
			_assetRepo = new AssetRepository(ConnectionString.Get(), productRepo);

			_log = LogManager.GetLogger(GetType());
		}

		private static AssetHistoryReadModel MapToModel(AssetHistoryDto input)
		{
			var model = new AssetHistoryReadModel();
			AssetsController.Populate(input, model);

			// Map the extra properties belonging only to the AssetHistoryDto.
			model.AssetId = input.AssetId;

			return model;
		}

		private static AssetHistoryQuery MapToQuery(AssetHistoryFilterModel filter) =>
			new AssetHistoryQuery
			{
				AssetId = filter.AssetId,
				Tag = filter.Tag
			};

		private IHttpActionResult CreateActionResultFor(IEnumerable<AssetHistoryDto> input)
		{
			if (!input.Any())
				return StatusCode(HttpStatusCode.NoContent);

			var models = input.Select(MapToModel);

			return Ok(models);
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
			[FromUri]AssetHistoryFilterModel filter)
		{
			filter = filter ?? new AssetHistoryFilterModel();
			IEnumerable<AssetHistoryDto> result;

			if (filter.IsEmpty())
			{
				result = _assetHistoryRepo.Query();

				return CreateActionResultFor(result);
			}

			var query = MapToQuery(filter);
			result = _assetHistoryRepo.Query(query);

			return CreateActionResultFor(result);
		}

		[Route("api/asset-histories/{id:long:min(1)/asset}")]
		public IHttpActionResult GetAssetFor(long id)
		{
			if (!_assetHistoryRepo.Exists(id))
				return NotFound();

			var historyDto = _assetHistoryRepo.GetById(id);
			var assetDto = _assetRepo.GetById(historyDto.AssetId);

			var readModel = AssetsController.MapToModel(assetDto);

			return Ok(readModel);
		}
	}
}