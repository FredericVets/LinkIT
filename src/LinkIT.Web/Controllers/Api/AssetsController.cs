using LinkIT.Data.DTO;
using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using LinkIT.Web.Infrastructure.Api;
using LinkIT.Web.Models.Api;
using LinkIT.Web.Models.Api.Filters;
using LinkIT.Web.Models.Api.Paging;
using log4net;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace LinkIT.Web.Controllers.Api
{
	public class AssetsController : ApiController
	{
		private const int MAX_NUMBER_OWNERS_ALLOWED = 50;

		private readonly IAssetRepository _repo;
		private readonly ILog _log;

		public AssetsController()
		{
			var productRepo = new ProductRepository(ConnectionString.Get());
			_repo = new AssetRepository(ConnectionString.Get(), productRepo);

			_log = LogManager.GetLogger(GetType());
		}

		private static AssetDto MapToDto(AssetWriteModel input, long? id = null) =>
			new AssetDto
			{
				Id = id,
				CreatedBy = input.CreatedBy,
				ModifiedBy = input.ModifiedBy,
				IctsReference = input.IctsReference,
				Tag = input.Tag,
				Serial = input.Serial,
				Product = new ProductDto { Id = input.ProductId.Value },
				Description = input.Description,
				InvoiceDate = input.InvoiceDate,
				InvoiceNumber = input.InvoiceNumber,
				Price = input.Price,
				PaidBy = input.PaidBy,
				Owner = input.Owner,
				InstallDate = input.InstallDate,
				InstalledBy = input.InstalledBy,
				Remark = input.Remark,
				TeamAsset = input.TeamAsset.Value
			};

		private static AssetQuery MapToQuery(AssetFilterModel filter) =>
			new AssetQuery
			{
				CreatedBy = filter.CreatedBy,
				ModifiedBy = filter.ModifiedBy,
				IctsReference = filter.IctsReference,
				Tag = filter.Tag,
				Serial = filter.Serial,
				ProductId = filter.ProductId,
				Description = filter.Description,
				InvoiceDate = filter.InvoiceDate,
				InvoiceNumber = filter.InvoiceNumber,
				Price = filter.Price,
				PaidBy = filter.PaidBy,
				Owner = filter.Owner,
				InstallDate = filter.InstallDate,
				InstalledBy = filter.InstalledBy,
				Remark = filter.Remark,
				TeamAsset = filter.TeamAsset
			};

		private IHttpActionResult CreateActionResultFor(PagedResult<AssetDto> pagedResult)
		{
			if (pagedResult.IsEmpty())
				return StatusCode(HttpStatusCode.NoContent);

			var models = pagedResult.Result.Select(MapToModel).ToList();
			var result = new PagedResultModel<AssetReadModel>(
				models,
				MappingHelper.MapToModel(pagedResult.PageInfo),
				pagedResult.TotalCount);

			return Ok(result);
		}

		public static AssetReadModel MapToModel(AssetDto input)
		{
			var model = new AssetReadModel();
			MappingHelper.PopulateReadModel(input, model);

			// Map the extra properties belonging only to the AssetReadModel.
			model.Product = ProductsController.MapToModel(input.Product);

			return model;
		}

		[Route("api/assets/{id:long:min(1)}", Name = "GetAssetById")]
		public IHttpActionResult Get(long id)
		{
			if (!_repo.Exists(id))
				return NotFound();

			var dto = _repo.GetById(id);
			var readModel = MapToModel(dto);

			return Ok(readModel);
		}

		[Route("api/assets")]
		public IHttpActionResult Get(
			[FromUri]AssetFilterModel filter,
			[FromUri]PageInfoModel pageInfo)
		{
			filter = filter ?? new AssetFilterModel();
			pageInfo = pageInfo ?? new PageInfoModel();
			PagedResult<AssetDto> pagedResult;

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
		/// Gets all the assets for a comma separated list of owners.
		/// </summary>
		/// <param name="owners">A comma separated list of owners.</param>
		/// <returns></returns>
		[Route("api/assets")]
		public IHttpActionResult GetForOwners(string owners)
		{
			if (string.IsNullOrWhiteSpace(owners))
				throw new ArgumentNullException(nameof(owners));

			var trimmed = owners
				.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(x => x.Trim())
				.Except(new[] { string.Empty })
				.ToList();
			if (trimmed.Count > MAX_NUMBER_OWNERS_ALLOWED)
				return BadRequest($"Maximum {MAX_NUMBER_OWNERS_ALLOWED} owners can be specified.");

			var dtos = _repo.ForOwners(trimmed);
			if (!dtos.Any())
				return StatusCode(HttpStatusCode.NoContent);
			
			var readModels = dtos.Select(MapToModel);

			return Ok(readModels);
		}

		[Route("api/assets/{id:long:min(1)}/product")]
		public IHttpActionResult GetProductFor(long id)
		{
			if (!_repo.Exists(id))
				return NotFound();

			var dto = _repo.GetById(id);
			var readModel = ProductsController.MapToModel(dto.Product);

			return Ok(readModel);
		}

		[Route("api/assets")]
		public IHttpActionResult Post(AssetWriteModel model)
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

			return CreatedAtRoute("GetAssetById", new { id = readModel.Id }, readModel);
		}

		// Fully updates the asset.
		[Route("api/assets/{id:long:min(1)}")]
		public IHttpActionResult Put(long id, AssetWriteModel model)
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

		[Route("api/assets/{id:long:min(1)}")]
		public IHttpActionResult Delete(long id)
		{
			if (!_repo.Exists(id))
				return NotFound();

			_repo.Delete(id);

			return StatusCode(HttpStatusCode.NoContent);
		}
	}
}