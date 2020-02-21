using System;
using System.Configuration;
using LinkIT.Data.DTO;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.DeviceRepo
{
	[TestClass]
	public class WhenGettingById
	{
		private Guid _id = new Guid("B25A5475-A799-4D19-99B0-1E0BFA4554E2");
		private DeviceDto _result;

		[TestInitialize]
		public void Setup()
		{
			var conStr = ConfigurationManager.ConnectionStrings["LinkITConnectionString"].ConnectionString;
			var repo = new DeviceRepository(conStr);

			_result = repo.GetById(_id);
		}

		[TestMethod]
		public void ThenTheResultIsAsExpected()
		{
			Assert.IsNotNull(_result);
			Assert.AreEqual(_id, _result.Id);
			Assert.AreEqual("CRD-L-04321", _result.Tag);
			Assert.AreEqual("u6543210", _result.Owner);
			Assert.AreEqual("HP", _result.Brand);
			Assert.AreEqual("EliteBook 745", _result.Type);
		}
	}
}