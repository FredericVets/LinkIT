using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Models.Api
{
	public class DeviceModel
	{
		[Range(0, long.MaxValue)]
		public long? Id { get; set; }

		[Required]
		[MaxLength(20)]
		public string Tag { get; set; }

		[Required]
		[MaxLength(20)]
		public string Owner { get; set; }

		[Required]
		[MaxLength(20)]
		public string Brand { get; set; }

		[Required]
		[MaxLength(20)]
		public string Type { get; set; }
	}
}