using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Models.Api
{
	public class ProductWriteModel
	{
		[MaxLength(30)]
		public string CreatedBy { get; set; }

		[MaxLength(30)]
		public string ModifiedBy { get; set; }

		[Required]
		[MaxLength(30)]
		public string Brand { get; set; }

		[Required]
		[MaxLength(30)]
		public string Type { get; set; }
	}
}