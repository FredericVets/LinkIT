using System;
using System.Configuration;
using LinkIT.Data.DTO;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.DeviceRepo
{
	[TestClass]
	public class WhenUpdatingADevice
	{
		private Guid _id = new Guid("B25A5475-A799-4D19-99B0-1E0BFA4554E2");

		// Subject Under Test.
		private DeviceRepository _sut;

		private DeviceDto _expected;
		private DeviceDto _actual;

		[TestInitialize]
		public void Setup()
		{
			var conStr = ConfigurationManager.ConnectionStrings["LinkITConnectionString"].ConnectionString;
			_sut = new DeviceRepository(conStr);

			_expected = _sut.GetById(_id);

			_expected.Brand = "Amazing";
			_expected.Type = "AwesomeBook";

			_sut.Update(_expected);
		}

		[TestMethod]
		public void ThenTheDataIsUpdatedAsExpected()
		{
			_actual = _sut.GetById(_id);

			Assert.AreEqual(_expected, _actual);
		}
	}
}
