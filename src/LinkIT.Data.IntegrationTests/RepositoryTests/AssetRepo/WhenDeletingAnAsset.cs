﻿using System;
using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.AssetRepo
{
	[TestClass]
	public class WhenDeletingAnAsset
	{
		private AssetDto _asset;
		private AssetRepository _sut;
		private ProductRepository _productRepo;

		private ProductDto InsertProduct()
		{
			var product = new ProductDto
			{
				CreatedBy = "user1",
				Brand = "HP",
				Type = "EliteBook",
				Group = "Laptop"
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

			_asset = new AssetDto
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

			_asset.Id = _sut.Insert(_asset);
		}

		[TestMethod]
		public void ThenTheAssetIsMarkedAsDeleted()
		{
			_sut.Delete(_asset.Id.Value);

			bool marked = new DatabaseHelper().IsAssetMarkedAsDeleted(_asset.Id.Value);
			Assert.IsTrue(marked);
		}

		[TestCleanup]
		public void Cleanup() =>
			new DatabaseHelper().HardDeleteAll();
	}
}