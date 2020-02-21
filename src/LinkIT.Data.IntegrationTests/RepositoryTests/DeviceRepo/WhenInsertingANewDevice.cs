using System;
using System.Configuration;
using LinkIT.Data.DTO;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.DeviceRepo
{
	[TestClass]
	public class WhenInsertingANewDevice
	{
		// Subject Under Test.
		private DeviceRepository _sut;

		private DeviceDto _expected;
		private DeviceDto _actual;

		[TestInitialize]
		public void Setup()
		{
			var conStr = ConfigurationManager.ConnectionStrings["LinkITConnectionString"].ConnectionString;
			_sut = new DeviceRepository(conStr);

			_expected = new DeviceDto
			{
				Id = Guid.NewGuid(),
				Brand = "Some brand",
				Type = "Some type",
				Owner = "John Doe",
				Tag = "Random"
			};

			_sut.Insert(_expected);
		}

		[TestMethod]
		public void ThenTheDataIsInserted()
		{
			_actual = _sut.GetById(_expected.Id.Value);

			Assert.AreEqual(_expected, _actual);
		}
	}
}
