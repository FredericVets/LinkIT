using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Models.Api.Filters
{
	public class DeviceFilter
	{
		[MaxLength(20)]
		public string Tag { get; set; }

		[MaxLength(20)]
		public string Owner { get; set; }

		[MaxLength(20)]
		public string Brand { get; set; }

		[MaxLength(20)]
		public string Type { get; set; }

		public bool IsEmpty()
		{
			return string.IsNullOrWhiteSpace(Tag) &&
				string.IsNullOrWhiteSpace(Owner) &&
				string.IsNullOrWhiteSpace(Brand) &&
				string.IsNullOrWhiteSpace(Type);
		}
	}
}