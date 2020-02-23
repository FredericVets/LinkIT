using LinkIT.Data.DTO;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.DeviceRepo
{
	[TestClass]
	public class WhenQueryingTheDevicesWithOneCondition
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
					Tag = "CRD-X-01234"
				},
				new DeviceDto
				{
					Brand = "Dell",
					Type = "Latitude",
					Owner = "Unknown",
					Tag = "CRD-X-43210"
				}
			};

			_expected.ForEach(x => x.Id = _sut.Insert(x));
		}

		[TestMethod]
		public void ThenTheResultIsAsExpected()
		{
			var actual = _sut.Query(new DeviceQuery { Owner = "Unknown" });

			Assert.AreEqual(2, actual.Count());
			foreach (var expectedDto in _expected)
			{
				var actualDto = actual.Single(x => x.Id == expectedDto.Id);
				Assert.AreEqual(expectedDto, actualDto);
			}
		}

		[TestCleanup]
		public void CleanUp()
		{
			_expected.ForEach(x => _sut.Delete(x.Id.Value));
		}
	}
}