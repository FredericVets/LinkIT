using System;
using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Models.Api.Filters
{
	public class AssetFilterModel
	{
		/// <summary>
		/// Filter by 'CreatedBy'.
		/// </summary>
		[MaxLength(30)]
		public string CreatedBy { get; set; }

		/// <summary>
		/// Filter by 'ModifiedBy'.
		/// </summary>
		[MaxLength(30)]
		public string ModifiedBy { get; set; }

		/// <summary>
		/// Filter by 'IctsReference'.
		/// </summary>
		[MaxLength(30)]
		public string IctsReference { get; set; }

		/// <summary>
		/// Filter by 'Tag'.
		/// </summary>
		[MaxLength(15)]
		public string Tag { get; set; }

		/// <summary>
		/// Filter by 'Serial'.
		/// </summary>
		[MaxLength(30)]
		public string Serial { get; set; }

		/// <summary>
		/// Filter by 'ProductId'.
		/// </summary>
		[Range(0, long.MaxValue)]
		public long? ProductId { get; set; }

		/// <summary>
		/// Filter by 'Description'.
		/// </summary>
		[MaxLength(500)]
		public string Description { get; set; }

		/// <summary>
		/// Filter by 'InvoiceDate'.
		/// </summary>
		public DateTime? InvoiceDate { get; set; }

		/// <summary>
		/// Filter by 'InvoiceNumber'.
		/// </summary>
		[MaxLength(30)]
		public string InvoiceNumber { get; set; }

		/// <summary>
		/// Filter by 'Price'.
		/// </summary>
		public decimal? Price { get; set; }

		/// <summary>
		/// Filter by 'PaidBy'.
		/// </summary>
		[MaxLength(30)]
		public string PaidBy { get; set; }

		/// <summary>
		/// Filter by 'Owner'.
		/// </summary>
		[MaxLength(30)]
		public string Owner { get; set; }

		/// <summary>
		/// Filter by 'InstallDate'.
		/// </summary>
		public DateTime? InstallDate { get; set; }

		/// <summary>
		/// Filter by 'InstalledBy'.
		/// </summary>
		[MaxLength(30)]
		public string InstalledBy { get; set; }

		/// <summary>
		/// Filter by 'Remark'.
		/// </summary>
		[MaxLength(500)]
		public string Remark { get; set; }

		/// <summary>
		/// Filter by 'TeamAsset'.
		/// </summary>
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