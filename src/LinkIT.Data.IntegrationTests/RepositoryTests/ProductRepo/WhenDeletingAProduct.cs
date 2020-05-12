using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.ProductRepo
{
	[TestClass]
	public class WhenDeletingAProduct
	{
		private ProductDto product;
		private ProductRepository _sut;

		[TestInitialize]
		public void Setup()
		{
			_sut = new ProductRepository(new ConnectionString());

			product = new ProductDto
			{
				CreatedBy = "user1",
				Brand = "HP",
				Type = "EliteBook",
				Group = "Laptop"
			};

			product.Id = _sut.Insert(product);
		}

		[TestMethod]
		public void ThenTheAssetIsMarkedAsDeleted()
		{
			Assert.ThrowsException<InvalidOperationException>(
				() => _sut.Delete(product.Id.Value),
				"A Product can not be deleted.");
		}

		[TestCleanup]
		public void Cleanup() =>
			new DatabaseHelper().HardDeleteAll();
	}
}