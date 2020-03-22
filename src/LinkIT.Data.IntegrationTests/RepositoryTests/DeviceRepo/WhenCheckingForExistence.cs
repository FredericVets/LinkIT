using LinkIT.Data.DTO;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.DeviceRepo
{
	[TestClass]
	public class WhenCheckingForExistence
	{
		private DeviceRepository _device;
		private DeviceDto _expected;

		[TestInitialize]
		public void Setup()
		{
			var conStr = ConfigurationManager.ConnectionStrings["LinkITConnectionString"].ConnectionString;
			_device = new DeviceRepository(conStr);

			_expected = new DeviceDto
			{
				Brand = "HP",
				Type = "AwesomeBook",
				Owner = "Unknown",
				Tag = "CRD-X-01234"
			};

			_expected.Id = _device.Insert(_expected);
		}

		[TestMethod]
		public void ThenTheDeviceExists()
		{
			bool actual = _device.Exists(_expected.Id.Value);
			Assert.IsTrue(actual);
		}

		[TestCleanup]
		public void Cleanup()
		{
			_device.Delete(_expected.Id.Value);
		}
	}
}