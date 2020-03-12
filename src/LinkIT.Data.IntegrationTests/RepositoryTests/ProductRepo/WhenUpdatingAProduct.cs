using System;
using System.Configuration;
using LinkIT.Data.DTO;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.ProductRepo
{
	[TestClass]
	public class WhenUpdatingAProduct
	{
		private ProductDto _expected;
		private ProductRepository _sut;

		[TestInitialize]
		public void Setup()
		{
			var conStr = ConfigurationManager.ConnectionStrings["LinkITConnectionString"].ConnectionString;
			_sut = new ProductRepository(conStr);

			_expected = new ProductDto
			{
				CreationDate = DateTime.Now.AddDays(-1),
				CreatedBy = "user1",
				ModificationDate = DateTime.Now,
				ModifiedBy = "user2",
				Brand = "HP",
				Type = "EliteBook"
			};

			_expected.Id = _sut.Insert(_expected);
		}

		[TestMethod]
		public void ThenTheDataIsInserted()
		{
			_expected.Brand = "Dell";
			_sut.Update(_expected);

			var actual = _sut.GetById(_expected.Id.Value);

			Assert.IsNotNull(actual);
			Assert.AreEqual(_expected, actual);
		}

		[TestCleanup]
		public void Cleanup()
		{
			_sut.Delete(_expected.Id.Value);
		}
	}
}