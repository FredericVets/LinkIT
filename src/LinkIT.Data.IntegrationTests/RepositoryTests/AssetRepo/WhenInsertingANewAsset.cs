using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.AssetRepo
{
	[TestClass]
	public class WhenInsertingANewAsset
	{
		private AssetDto _expected;
		private AssetRepository _sut;
		private ProductRepository _productRepo;

		private ProductDto InsertProduct()
		{
			var product = new ProductDto
			{
				CreatedBy = "user1",
				Brand = "HP",
				Type = "EliteBook"
			};

			product.Id = _productRepo.Insert(product);

			return product;
		}

		[TestInitialize]
		public void Setup()
		{
			var connString = new ConnectionString();
			_productRepo = new ProductRepository(connString);
			_sut = new AssetRepository(connString, _productRepo);

			var product = InsertProduct();

			_expected = new AssetDto
			{
				CreatedBy = "user2",
				Tag = "CRD-X-012345",
				Serial = "xx0123456789",
				Product = product,
				Description = "Asset Description",
				InvoiceDate = DateTime.Now.AddDays(-7),
				InvoiceNumber = "ii0123456789",
				Price = 50.01M,
				PaidBy = "user3",
				Owner = "user4",
				Remark = "Not yet installed",
				TeamAsset = false
			};
		}

		[TestMethod]
		public void ThenTheAssetIsInserted()
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