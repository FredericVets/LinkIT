using LinkIT.Data.DTO;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.DeviceRepo
{
	[TestClass]
	public class WhenGettingAllDevices
	{
		private IEnumerable<DeviceDto> _result;

		[TestInitialize]
		public void Setup()
		{
			var conStr = ConfigurationManager.ConnectionStrings["LinkITConnectionString"].ConnectionString;
			var repo = new DeviceRepository(conStr);

			_result = repo.GetAll();
		}

		[TestMethod]
		public void ThenTheResultIsAsExpected()
		{
			Assert.IsNotNull(_result);
		}
	}
}