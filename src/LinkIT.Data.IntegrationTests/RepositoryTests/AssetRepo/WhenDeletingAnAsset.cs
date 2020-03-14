using System;
using LinkIT.Data.DTO;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.AssetRepo
{
	[TestClass]
	public class WhenDeletingAnAsset
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

			return _productRepo.GetById(product.Id.Value);
		}

		[TestInitialize]
		public void Setup()
		{
			_productRepo = new ProductRepository(AssetDatabaseHelper.ConnectionString);
			_sut = new AssetRepository(AssetDatabaseHelper.ConnectionString, _productRepo);

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
				Price = 50M,
				PaidBy = "user3",
				Owner = "user4",
				Remark = "Not yet installed",
				TeamAsset = false
			};

			_expected.Id = _sut.Insert(_expected);
		}

		[TestMethod]
		public void ThenTheAssetIsMarkedAsDeleted()
		{
			_sut.Delete(_expected.Id.Value);

			bool marked = AssetDatabaseHelper.IsMarkedAsDeleted(_expected.Id.Value);
			Assert.IsTrue(marked);
		}

		[TestCleanup]
		public void Cleanup()
		{
			AssetDatabaseHelper.HardDelete(_expected.Id.Value);
			_productRepo.Delete(_expected.Product.Id.Value);
		}
	}
}