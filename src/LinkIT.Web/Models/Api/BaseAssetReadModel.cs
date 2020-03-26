using System;

namespace LinkIT.Web.Models.Api
{
	public abstract class BaseAssetReadModel
	{
		public long Id { get; set; }

		public DateTime CreationDate { get; set; }

		public string CreatedBy { get; set; }

		public DateTime ModificationDate { get; set; }

		public string ModifiedBy { get; set; }

		public string IctsReference { get; set; }

		public string Tag { get; set; }

		public string Serial { get; set; }

		public string Description { get; set; }

		public DateTime? InvoiceDate { get; set; }

		public string InvoiceNumber { get; set; }

		public decimal? Price { get; set; }

		public string PaidBy { get; set; }

		public string Owner { get; set; }

		public DateTime? InstallDate { get; set; }

		public string InstalledBy { get; set; }

		public string Remark { get; set; }

		public bool TeamAsset { get; set; }
	}
}