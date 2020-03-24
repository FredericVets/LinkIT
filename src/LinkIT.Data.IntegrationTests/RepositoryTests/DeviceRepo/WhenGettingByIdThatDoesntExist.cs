using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.DeviceRepo
{
	[TestClass]
	public class WhenGettingByIdThatDoesntExist
	{
		private DeviceRepository _sut;

		[TestInitialize]
		public void Setup()
		{
			_sut = new DeviceRepository(ConnectionString.Get());
		}

		[TestMethod]
		public void ThenAnExceptionIsThrown()
		{
			DeviceDto actual = null;
			Assert.ThrowsException<ArgumentException>(
				() => actual = _sut.GetById(-1),
				"Not all supplied id's exist.");

			Assert.IsNull(actual);
		}
	}
}