using System;
using System.Configuration;
using LinkIT.Data.DTO;
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
			var conStr = ConfigurationManager.ConnectionStrings["LinkITConnectionString"].ConnectionString;
			_sut = new DeviceRepository(conStr);

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