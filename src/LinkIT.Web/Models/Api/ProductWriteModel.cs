using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Models.Api
{
	public class ProductWriteModel
	{
		[Required]
		[MaxLength(30)]
		public string Brand { get; set; }

		[Required]
		[MaxLength(30)]
		public string Type { get; set; }

		[Required]
		[MaxLength(30)]
		public string Group { get; set; }
	}
}