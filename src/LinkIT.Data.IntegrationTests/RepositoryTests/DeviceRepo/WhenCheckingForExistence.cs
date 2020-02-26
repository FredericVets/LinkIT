using LinkIT.Data.DTO;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.DeviceRepo
{
	[TestClass]
	public class WhenCheckingForExistence
	{
		private DeviceRepository _sut;
		private DeviceDto _expected;

		[TestInitialize]
		public void Setup()
		{
			var conStr = ConfigurationManager.ConnectionStrings["LinkITConnectionString"].ConnectionString;
			_sut = new DeviceRepository(conStr);

			_expected = new DeviceDto
			{
				Brand = "HP",
				Type = "AwesomeBook",
				Owner = "Unknown",
				Tag = "CRD-X-01234"
			};

			_expected.Id = _sut.Insert(_expected);
		}

		[TestMethod]
		public void ThenTheDeviceDoesntExist()
		{
			bool actual = _sut.Exists(_expected.Id.Value);
			Assert.IsTrue(actual);
		}

		[TestCleanup]
		public void Cleanup()
		{
			_sut.Delete(_expected.Id.Value);
		}
	}
}