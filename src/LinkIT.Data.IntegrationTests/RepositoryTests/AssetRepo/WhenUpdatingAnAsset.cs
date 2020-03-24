using System;
using System.Collections.Generic;
using System.Linq;
using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.AssetRepo
{
	[TestClass]
	public class WhenUpdatingAnAsset
	{
		private List<AssetDto> _expected;
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
			_productRepo = new ProductRepository(ConnectionString.Get());
			_sut = new AssetRepository(ConnectionString.Get(), _productRepo);

			var product = InsertProduct();

			_expected = new List<AssetDto>
			{
				new AssetDto
				{
					CreatedBy = "user1",
					Tag = "CRD-X-00001",
					Product = product,
					PaidBy = "user1",
					Owner = "user1",
					TeamAsset = true
				},
				new AssetDto
				{
					CreatedBy = "user2",
					Tag = "CRD-X-00002",
					Serial = "xx0123456789",
					Product = product,
					Description = "Asset Description",
					InvoiceDate = DateTime.Now.AddDays(-7),
					InvoiceNumber = "ii0123456789",
					Price = 50.14M,
					PaidBy = "user2",
					Owner = "user2",
					InstallDate = DateTime.Now.AddDays(2),
					InstalledBy = "user2",
					Remark = "To be installed within 2 days",
					TeamAsset = false
				}
			};

			_expected.ForEach(x => x.Id = _sut.Insert(x));
		}

		[TestMethod]
		public void ThenTheAssetsAreUpdated()
		{
			_expected[0].Tag = "CRD-X-00003";
			_expected[0].ModifiedBy = "user3";
			_expected[1].Description = "Modified description";
			_expected[1].ModifiedBy = "user3";

			_expected.ForEach(_sut.Update);

			foreach (var asset in _expected)
			{
				var actual = _sut.GetById(asset.Id.Value);
				Assert.AreEqual(asset, actual);
			}
		}

		[TestCleanup]
		public void CleanUp()
		{
			new DatabaseHelper().HardDeleteAll();
		}
	}
}