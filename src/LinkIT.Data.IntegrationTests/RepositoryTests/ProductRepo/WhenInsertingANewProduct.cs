using System;
using System.Configuration;
using LinkIT.Data.DTO;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.ProductRepo
{
	[TestClass]
	public class WhenInsertingANewProduct
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
				CreatedBy = "user1",
				Brand = "HP",
				Type = "EliteBook"
			};
		}

		[TestMethod]
		public void ThenTheDataIsInserted()
		{
			var created = DateTime.Now;
			DateTimeProvider.SetDateTime(created);

			_expected.Id = _sut.Insert(_expected);
			_expected.CreationDate = _expected.ModificationDate = created;
			_expected.ModifiedBy = _expected.CreatedBy;

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
