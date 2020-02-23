using LinkIT.Data.DTO;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.DeviceRepo
{
	[TestClass]
	public class WhenGettingAllDevicesWithPaging
	{
		private List<DeviceDto> _expected;
		private DeviceRepository _sut;

		[TestInitialize]
		public void Setup()
		{
			var conStr = ConfigurationManager.ConnectionStrings["LinkITConnectionString"].ConnectionString;
			_sut = new DeviceRepository(conStr);

			_expected = new List<DeviceDto>()
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

			_expected.ForEach(x => x.Id = _sut.Insert(x));
		}

		[TestMethod]
		public void ThenTheResultIsAsExpected()
		{
			var paging = new Paging(2, 2, DeviceRepository.TAG_COLUMN);
			var actual = _sut.Query(paging: paging).ToList();

			var page = _expected.OrderBy(x => x.Tag).Skip(2).ToList();

			Assert.AreEqual(2, actual.Count);
			foreach (var item in page)
			{
				var actualDto = actual.Single(x => x.Id == item.Id);
				Assert.AreEqual(item, actualDto);
			}
		}

		[TestCleanup]
		public void CleanUp()
		{
			_expected.ForEach(x => _sut.Delete(x.Id.Value));
		}
	}
}