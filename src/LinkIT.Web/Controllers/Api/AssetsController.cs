using LinkIT.Data.DTO;
using LinkIT.Data.Extensions;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace LinkIT.Web.Controllers.Api
{
	public class AssetsController : ApiController
	{
		private const int MAX_NUMBER_OWNERS_ALLOWED = 25;
		private const string UNKNOWN_PRODUCT = "Unknown product.";

		private readonly IAssetRepository _assetRepo;
		private readonly IRepository<ProductDto, ProductQuery> _productRepo;
		private readonly IJsonWebTokenWrapper _jwt;
		private readonly ILog _log;

		public AssetsController(
			IAssetRepository assetRepo,
			IRepository<ProductDto, ProductQuery> productRepo,
			IJsonWebTokenWrapper jwt)
		{
			_assetRepo = assetRepo;
			_productRepo = productRepo;
			_jwt = jwt;
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

		/// <summary>
		/// Gets the assets that match the filter criteria in a paging fashion.
		/// Filter criteria are optional.
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="pageInfo"></param>
		/// <returns></returns>
		[Route("api/assets")]
		[JwtAuthorize(Roles = Constants.Roles.READ)]
		[SwaggerResponse(HttpStatusCode.OK,
			Type = typeof(PagedResultModel<AssetReadModel>),
			Description = Consts.SWAGGER_PAGING_RESPONSE_DESCRIPTION)]
		[SwaggerResponse(HttpStatusCode.NoContent)]
		[SwaggerResponse(HttpStatusCode.BadRequest)]
		[SwaggerResponse(HttpStatusCode.Unauthorized)]
		public IHttpActionResult Get(
			[FromUri] AssetFilterModel filter,
			[FromUri] PageInfoModel pageInfo)
		{
			filter = filter ?? new AssetFilterModel();
			pageInfo = pageInfo ?? new PageInfoModel();
			PagedResult<AssetDto> pagedResult;

			var paging = MappingHelper.MapToPageInfo(pageInfo);
			if (!paging.OrderBy.IsValidFor(_assetRepo.Columns))
				return BadRequest($"Unknown field : {paging.OrderBy.Name}.");

			if (filter.IsEmpty())
			{
				pagedResult = _assetRepo.PagedQuery(paging);

				return CreateActionResultFor(pagedResult);
			}

			// Apply filter.
			var query = MapToQuery(filter);
			pagedResult = _assetRepo.PagedQuery(paging, query);

			return CreateActionResultFor(pagedResult);
		}

		/// <summary>
		/// Gets the asset with the specified id.
		/// </summary>
		/// <param name="id">The id of the asset that is to be retrieved.</param>
		/// <returns></returns>
		[Route("api/assets/{id:long:min(1)}", Name = "GetAssetById")]
		[JwtAuthorize(Roles = Constants.Roles.READ)]
		[SwaggerResponse(HttpStatusCode.OK, Type = typeof(AssetReadModel))]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[SwaggerResponse(HttpStatusCode.Unauthorized)]
		public IHttpActionResult GetAssetById(long id)
		{
			if (!_assetRepo.Exists(id))
				return NotFound();

			var dto = _assetRepo.GetById(id);
			var readModel = MapToModel(dto);

			return Ok(readModel);
		}

		/// <summary>
		/// Gets all the assets for a comma separated list of owners.
		/// No paging is applied.
		/// </summary>
		/// <param name="owners">A comma separated list of owners. Maximum 25 owners can be specified.</param>
		/// <returns></returns>
		[Route("api/assets/of-owners")]
		[JwtAuthorize(Roles = Constants.Roles.READ)]
		[SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<AssetReadModel>))]
		[SwaggerResponse(HttpStatusCode.NoContent)]
		[SwaggerResponse(HttpStatusCode.BadRequest)]
		[SwaggerResponse(HttpStatusCode.Unauthorized)]
		public IHttpActionResult GetForOwners(string owners)
		{
			// Webapi will make sure that owners is not null or whitespace.
			var splitted = owners.SplitCommaSeparated();
			if (splitted.Length > MAX_NUMBER_OWNERS_ALLOWED)
				return BadRequest($"Maximum {MAX_NUMBER_OWNERS_ALLOWED} owners can be specified.");

			var dtos = _assetRepo.ForOwners(splitted);
			if (!dtos.Any())
				return StatusCode(HttpStatusCode.NoContent);

			var readModels = dtos.Select(MapToModel);

			return Ok(readModels);
		}

		/// <summary>
		/// Gets the product referenced by the asset with the specified id.
		/// </summary>
		/// <param name="id">The id of the asset that is to be retrieved.</param>
		/// <returns></returns>
		[Route("api/assets/{id:long:min(1)}/product")]
		[JwtAuthorize(Roles = Constants.Roles.READ)]
		[SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProductReadModel))]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[SwaggerResponse(HttpStatusCode.Unauthorized)]
		public IHttpActionResult GetProductFor(long id)
		{
			if (!_assetRepo.Exists(id))
				return NotFound();

			var dto = _assetRepo.GetById(id);
			var readModel = ProductsController.MapToModel(dto.Product);

			return Ok(readModel);
		}

		/// <summary>
		/// Creates a new asset.
		/// </summary>
		/// <param name="model">The new asset.</param>
		/// <returns></returns>
		[Route("api/assets")]
		[JwtAuthorize(Roles = Constants.Roles.CREATE)]
		[SwaggerResponse(HttpStatusCode.Created, Type = typeof(AssetReadModel))]
		[SwaggerResponse(HttpStatusCode.BadRequest)]
		[SwaggerResponse(HttpStatusCode.Unauthorized)]
		public IHttpActionResult Post(AssetWriteModel model)
		{
			if (model == null)
				return BadRequest(Constants.MISSING_MESSAGE_BODY);

			if (!_productRepo.Exists(model.ProductId.Value))
				return BadRequest(UNKNOWN_PRODUCT);

			var dto = MapToDto(model, createdBy: _jwt.UserId);
			long id = _assetRepo.Insert(dto);

			// Refetch the data.
			dto = _assetRepo.GetById(id);
			var readModel = MapToModel(dto);

			return CreatedAtRoute(nameof(GetAssetById), new { id = readModel.Id }, readModel);
		}

		/// <summary>
		/// Fully updates the asset with the specified id.
		/// </summary>
		/// <param name="id">The id of the asset that is to be updated.</param>
		/// <param name="model">The updated asset.</param>
		/// <returns></returns>
		[Route("api/assets/{id:long:min(1)}")]
		[JwtAuthorize(Roles = Constants.Roles.MODIFY)]
		[SwaggerResponse(HttpStatusCode.Created, Type = typeof(AssetReadModel))]
		[SwaggerResponse(HttpStatusCode.BadRequest)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[SwaggerResponse(HttpStatusCode.Unauthorized)]
		public IHttpActionResult Put(long id, AssetWriteModel model)
		{
			if (model == null)
				return BadRequest(Constants.MISSING_MESSAGE_BODY);

			if (!_assetRepo.Exists(id))
				return NotFound();

			if (!_productRepo.Exists(model.ProductId.Value))
				return BadRequest(UNKNOWN_PRODUCT);

			var dto = MapToDto(model, id, modifiedBy: _jwt.UserId);

			_assetRepo.Update(dto);

			// Refetch the data.
			dto = _assetRepo.GetById(id);
			var readModel = MapToModel(dto);

			return Ok(readModel);
		}
		/// <summary>
		/// Deletes the asset with the specified id.
		/// Performs a soft-delete : sets the Deleted column to 1.
		/// </summary>
		/// <param name="id">The id of the asset that is to be deleted.</param>
		/// <returns></returns>
		[Route("api/assets/{id:long:min(1)}")]
		[JwtAuthorize(Roles = Constants.Roles.DELETE)]
		[SwaggerResponse(HttpStatusCode.NoContent)]
		[SwaggerResponse(HttpStatusCode.NotFound)]
		[SwaggerResponse(HttpStatusCode.Unauthorized)]
		public IHttpActionResult Delete(long id)
		{
			if (!_assetRepo.Exists(id))
				return NotFound();

			_assetRepo.Delete(id);

			return StatusCode(HttpStatusCode.NoContent);
		}
	}
}