using System;

namespace LinkIT.Web.Models
{
	public class DeviceModel
	{
		public Guid? Id { get; set; }

		public string Tag { get; set; }

		public string Owner { get; set; }

		public string Brand { get; set; }

		public string Type { get; set; }
	}
}