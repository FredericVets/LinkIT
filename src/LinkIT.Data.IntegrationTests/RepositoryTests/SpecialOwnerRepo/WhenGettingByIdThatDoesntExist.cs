using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.SpecialOwnerRepo
{
	[TestClass]
	public class WhenGettingByIdThatDoesntExist
	{
		private SpecialOwnerRepository _sut;

		[TestInitialize]
		public void Setup() =>
			_sut = new SpecialOwnerRepository(ConnectionString.Get());

		[TestMethod]
		public void ThenAnExceptionIsThrown()
		{
			Assert.ThrowsException<ArgumentException>(
				() => _sut.GetById(-1),
				"Not all supplied id's exist.");
		}
	}
}
