using LinkIT.Data.DTO;
using LinkIT.Data.Paging;
using LinkIT.Web.Models.Api;
using LinkIT.Web.Models.Api.Paging;
using System;

namespace LinkIT.Web.Controllers.Api
{
	public static class MappingHelper
	{
		public static PageInfo MapToPageInfo(PageInfoModel input) =>
			new PageInfo(
				input.PageNumber,
				input.RowsPerPage,
				input.OrderBy);

		public static PageInfoModel MapToModel(PageInfo input) =>
			new PageInfoModel
			{
				PageNumber = input.PageNumber,
				RowsPerPage = input.RowsPerPage,
				OrderBy = input.OrderBy
			};

		public static void PopulateReadModel(AssetDto from, BaseAssetReadModel to)
		{
			if (from == null)
				throw new ArgumentNullException(nameof(from));
			if (to == null)
				throw new ArgumentNullException(nameof(to));

			to.Id = from.Id.Value;
			to.CreationDate = from.CreationDate.Value;
			to.CreatedBy = from.CreatedBy;
			to.ModificationDate = from.ModificationDate.Value;
			to.ModifiedBy = from.ModifiedBy;
			to.IctsReference = from.IctsReference;
			to.Tag = from.Tag;
			to.Serial = from.Serial;
			to.Description = from.Description;
			to.InvoiceDate = from.InvoiceDate;
			to.InvoiceNumber = from.InvoiceNumber;
			to.Price = from.Price;
			to.PaidBy = from.PaidBy;
			to.Owner = from.Owner;
			to.InstallDate = from.InstallDate;
			to.InstalledBy = from.InstalledBy;
			to.Remark = from.Remark;
			to.TeamAsset = from.TeamAsset;
		}
	}
}