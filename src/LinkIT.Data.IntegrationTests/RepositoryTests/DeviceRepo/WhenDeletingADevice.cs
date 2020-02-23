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
		public void ThenTheDeviceDoesntExistAnymore()
		{
			_sut.Delete(_expected.Id.Value);

			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				_sut.Get(_expected.Id.Value);
			});
		}
	}
}