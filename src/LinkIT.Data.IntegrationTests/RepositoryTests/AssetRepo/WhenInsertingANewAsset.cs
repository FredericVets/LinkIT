using LinkIT.Data.DTO;
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
		}

		[TestMethod]
		public void ThenTheAssetIsInserted()
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
			AssetDatabaseHelper.HardDelete(_expected.Id.Value);
			_productRepo.Delete(_expected.Product.Id.Value);
		}
	}
}