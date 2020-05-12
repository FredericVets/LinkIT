using System;

namespace LinkIT.Web.Models.Api
{
	public class ProductReadModel
	{
		public long Id { get; set; }

		public DateTime CreationDate { get; set; }

		public string CreatedBy { get; set; }

		public DateTime ModificationDate { get; set; }

		public string ModifiedBy { get; set; }

		public string Brand { get; set; }

		public string Type { get; set; }

		public string Group { get; set; }
	}
}