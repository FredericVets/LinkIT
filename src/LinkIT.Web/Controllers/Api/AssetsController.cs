using LinkIT.Data;
using LinkIT.Data.DTO;
using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using LinkIT.Web.Filters.Api;
using LinkIT.Web.Infrastructure.Api.Shibboleth;
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
		private readonly ShibbolethAttributes _shibbolethAttribs;
		private readonly ILog _log;

		public AssetsController(IAssetRepository repo, ShibbolethAttributes shibbolethAttribs)
		{
			_repo = repo;
			_shibbolethAttribs = shibbolethAttribs;
			_log = LogManager.GetLogger(GetType());
		}

		private static AssetDto MapToDto(
			AssetWriteModel input, long? id = null, string createdBy = null, string modifiedBy = null) =>
			new AssetDto
			{
				Id = id,
				CreatedBy = createdBy,
				ModifiedBy = modifiedBy,
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
		[ShibbolethAuthorize(Roles = Constants.Roles.READ)]
		public IHttpActionResult Get(long id)
		{
			if (!_repo.Exists(id))
				return NotFound();

			var dto = _repo.GetById(id);
			var readModel = MapToModel(dto);

			return Ok(readModel);
		}

		[Route("api/assets")]
		[ShibbolethAuthorize(Roles = Constants.Roles.READ)]
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
		[ShibbolethAuthorize(Roles = Constants.Roles.READ)]
		public IHttpActionResult GetForOwners(string owners)
		{
			if (string.IsNullOrWhiteSpace(owners))
				throw new ArgumentNullException(nameof(owners));

			var splitted = owners.SplitCommaSeparated();
			if (splitted.Length > MAX_NUMBER_OWNERS_ALLOWED)
				return BadRequest($"Maximum {MAX_NUMBER_OWNERS_ALLOWED} owners can be specified.");

			var dtos = _repo.ForOwners(splitted);
			if (!dtos.Any())
				return StatusCode(HttpStatusCode.NoContent);
			
			var readModels = dtos.Select(MapToModel);

			return Ok(readModels);
		}

		[Route("api/assets/{id:long:min(1)}/product")]
		[ShibbolethAuthorize(Roles = Constants.Roles.READ)]
		public IHttpActionResult GetProductFor(long id)
		{
			if (!_repo.Exists(id))
				return NotFound();

			var dto = _repo.GetById(id);
			var readModel = ProductsController.MapToModel(dto.Product);

			return Ok(readModel);
		}

		[Route("api/assets")]
		[ShibbolethAuthorize(Roles = Constants.Roles.CREATE)]
		public IHttpActionResult Post(AssetWriteModel model)
		{
			if (model == null)
				return BadRequest(Constants.MISSING_MESSAGE_BODY);

			var dto = MapToDto(model, createdBy: _shibbolethAttribs.GetUid());
			long id = _repo.Insert(dto);

			// Refetch the data.
			dto = _repo.GetById(id);
			var readModel = MapToModel(dto);

			return CreatedAtRoute("GetAssetById", new { id = readModel.Id }, readModel);
		}

		// Fully updates the asset.
		[Route("api/assets/{id:long:min(1)}")]
		[ShibbolethAuthorize(Roles = Constants.Roles.MODIFY)]
		public IHttpActionResult Put(long id, AssetWriteModel model)
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

		[Route("api/assets/{id:long:min(1)}")]
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