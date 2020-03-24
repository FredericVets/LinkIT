using System;
using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Models.Api.Filters
{
	public class AssetFilterModel
	{
		[MaxLength(30)]
		public string CreatedBy { get; set; }

		[MaxLength(30)]
		public string ModifiedBy { get; set; }

		[MaxLength(30)]
		public string IctsReference { get; set; }

		[MaxLength(15)]
		public string Tag { get; set; }

		[MaxLength(30)]
		public string Serial { get; set; }

		[Range(0, long.MaxValue)]
		public long? ProductId { get; set; }

		[MaxLength(500)]
		public string Description { get; set; }

		public DateTime? InvoiceDate { get; set; }

		[MaxLength(30)]
		public string InvoiceNumber { get; set; }

		public decimal? Price { get; set; }

		[MaxLength(30)]
		public string PaidBy { get; set; }

		[MaxLength(30)]
		public string Owner { get; set; }

		public DateTime? InstallDate { get; set; }

		[MaxLength(30)]
		public string InstalledBy { get; set; }

		[MaxLength(500)]
		public string Remark { get; set; }

		public bool? TeamAsset { get; set; }

		public bool IsEmpty() =>
			string.IsNullOrWhiteSpace(CreatedBy) &&
			string.IsNullOrWhiteSpace(ModifiedBy) &&
			string.IsNullOrWhiteSpace(IctsReference) &&
			string.IsNullOrWhiteSpace(Tag) &&
			string.IsNullOrWhiteSpace(Serial) &&
			!ProductId.HasValue &&
			string.IsNullOrWhiteSpace(Description) &&
			!InvoiceDate.HasValue &&
			string.IsNullOrWhiteSpace(InvoiceNumber) &&
			!Price.HasValue &&
			string.IsNullOrWhiteSpace(PaidBy) &&
			string.IsNullOrWhiteSpace(Owner) &&
			!InstallDate.HasValue &&
			string.IsNullOrWhiteSpace(InstalledBy) &&
			string.IsNullOrWhiteSpace(Remark) &&
			!TeamAsset.HasValue;
	}
}