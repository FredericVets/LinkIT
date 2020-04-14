using System;
using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Models.Api
{
	public class AssetWriteModel
	{
		[MaxLength(30)]
		public string IctsReference { get; set; }

		[Required]
		[MaxLength(15)]
		public string Tag { get; set; }

		[MaxLength(30)]
		public string Serial { get; set; }

		[Required]
		[Range(0, long.MaxValue)]
		public long? ProductId { get; set; }

		[MaxLength(500)]
		public string Description { get; set; }

		public DateTime? InvoiceDate { get; set; }

		[MaxLength(30)]
		public string InvoiceNumber { get; set; }

		public decimal? Price { get; set; }

		[Required]
		[MaxLength(30)]
		public string PaidBy { get; set; }

		[Required]
		[MaxLength(30)]
		public string Owner { get; set; }

		public DateTime? InstallDate { get; set; }

		[MaxLength(30)]
		public string InstalledBy { get; set; }

		[MaxLength(500)]
		public string Remark { get; set; }

		[Required]
		public bool? TeamAsset { get; set; }
	}
}