using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.DeviceRepo
{
	[TestClass]
	public class WhenDeletingADevice
	{
		private DeviceDto _device;
		private DeviceRepository _sut;

		[TestInitialize]
		public void Setup()
		{
			_sut = new DeviceRepository(ConnectionString.Get());

			_device = new DeviceDto
			{
				Brand = "HP",
				Type = "AwesomeBook",
				Owner = "Unknown",
				Tag = "CRD-X-01234"
			};

			_device.Id = _sut.Insert(_device);
		}

		[TestMethod]
		public void ThenTheDeviceDoesntExistAnymore()
		{
			_sut.Delete(_device.Id.Value);

			Assert.IsFalse(_sut.Exists(_device.Id.Value));
		}
	}
}