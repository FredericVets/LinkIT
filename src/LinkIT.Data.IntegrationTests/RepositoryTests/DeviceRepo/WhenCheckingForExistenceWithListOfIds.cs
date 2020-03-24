using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.DeviceRepo
{
	[TestClass]
	public class WhenCheckingForExistenceWithListOfIds
	{
		private List<DeviceDto> _devices;
		private DeviceRepository _sut;

		[TestInitialize]
		public void Setup()
		{
			_sut = new DeviceRepository(ConnectionString.Get());

			_devices = new List<DeviceDto>()
			{
				new DeviceDto
				{
					Brand = "HP",
					Type = "AwesomeBook",
					Owner = "Unknown",
					Tag = "CRD-X-11111"
				},
				new DeviceDto
				{
					Brand = "Dell",
					Type = "Latitude 7290",
					Owner = "Unknown",
					Tag = "CRD-X-22222"
				},
				new DeviceDto
				{
					Brand = "Dell",
					Type = "Latitude 7490",
					Owner = "Unknown",
					Tag = "CRD-X-33333"
				},
				new DeviceDto
				{
					Brand = "Dell",
					Type = "Latitude 5590",
					Owner = "Unknown",
					Tag = "CRD-X-44444"
				}
			};

			_devices.ForEach(x => x.Id = _sut.Insert(x));
		}

		[TestMethod]
		public void ThenTheDevicesExist()
		{
			var ids = _devices.Select(x => x.Id.Value);
			bool actual = _sut.Exists(ids);

			Assert.IsTrue(actual);
		}

		[TestCleanup]
		public void CleanUp()
		{
			_devices.ForEach(x => _sut.Delete(x.Id.Value));
		}
	}
}