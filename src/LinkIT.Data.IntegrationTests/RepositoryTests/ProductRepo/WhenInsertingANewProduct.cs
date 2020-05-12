using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
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
			_sut = new ProductRepository(new ConnectionString());

			_expected = new ProductDto
			{
				CreatedBy = "user1",
				Brand = "HP",
				Type = "EliteBook",
				Group = "Laptop"
			};
		}

		[TestMethod]
		public void ThenTheDataIsInserted()
		{
			_expected.Id = _sut.Insert(_expected);
			var actual = _sut.GetById(_expected.Id.Value);

			Assert.IsNotNull(actual);
			Assert.AreEqual(_expected, actual);
		}

		[TestCleanup]
		public void Cleanup() =>
			new DatabaseHelper().HardDeleteAll();
	}
}