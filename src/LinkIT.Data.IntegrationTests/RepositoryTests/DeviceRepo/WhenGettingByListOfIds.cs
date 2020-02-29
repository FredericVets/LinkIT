using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using LinkIT.Data.DTO;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.DeviceRepo
{
	[TestClass]
	public class WhenGettingByListOfIds
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
			var actual = _sut.GetById(_expected.Select(x => x.Id.Value)).ToList();

			Assert.AreEqual(_expected.Count, actual.Count);
			foreach (var item in _expected)
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