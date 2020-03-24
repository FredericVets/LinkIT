using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
			_sut = new DeviceRepository(ConnectionString.Get());

			_expected = new DeviceDto
			{
				Brand = "HP",
				Type = "AwesomeBook",
				Owner = "Unknown",
				Tag = "CRD-X-01234"
			};
		}

		[TestMethod]
		public void ThenTheDataIsInserted()
		{
			_expected.Id = _sut.Insert(_expected);

			var actual = _sut.GetById(_expected.Id.Value);

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