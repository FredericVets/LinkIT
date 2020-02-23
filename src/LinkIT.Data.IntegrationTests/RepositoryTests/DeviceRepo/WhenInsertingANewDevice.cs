using LinkIT.Data.DTO;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.DeviceRepo
{
	[TestClass]
	public class WhenInsertingANewDevice
	{
		private DeviceDto _expected;
		private DeviceRepository _sut;

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
		public void ThenTheDataIsInserted()
		{
			var actual = _sut.Get(_expected.Id.Value);

			Assert.IsNotNull(actual);
			Assert.AreEqual(_expected, actual);
		}

		[TestCleanup]
		public void Cleanup()
		{
			_sut.Delete(_expected.Id.Value);
		}
	}
}