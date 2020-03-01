using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Models.Api.Filters
{
	public class DeviceFilterModel
	{
		[MaxLength(20)]
		public string Brand { get; set; }

		[MaxLength(20)]
		public string Type { get; set; }

		[MaxLength(20)]
		public string Owner { get; set; }

		[MaxLength(20)]
		public string Tag { get; set; }

		public bool IsEmpty()
		{
			return string.IsNullOrWhiteSpace(Brand) &&
				string.IsNullOrWhiteSpace(Type) &&
				string.IsNullOrWhiteSpace(Owner) &&
				string.IsNullOrWhiteSpace(Tag);
		}
	}
}