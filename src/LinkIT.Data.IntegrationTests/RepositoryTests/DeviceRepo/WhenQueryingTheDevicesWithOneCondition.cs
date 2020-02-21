using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using LinkIT.Data.DTO;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.DeviceRepo
{
	[TestClass]
	public class WhenQueryingTheDevicesWithOneCondition
	{
		private IEnumerable<DeviceDto> _result;

		[TestInitialize]
		public void Setup()
		{
			var conStr = ConfigurationManager.ConnectionStrings["LinkITConnectionString"].ConnectionString;
			var repo = new DeviceRepository(conStr);

			_result = repo.Query(new DeviceQuery { Tag = "CRD-L-04321" });
		}

		[TestMethod]
		public void ThenTheResultIsAsExpected()
		{
			Assert.IsNotNull(_result);
			Assert.AreEqual(1, _result.Count());
		}
	}
}