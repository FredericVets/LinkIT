using LinkIT.Data.DTO;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.AssetRepo
{
	[TestClass]
	public class WhenGettingTheAssetsForOwners
	{
		private List<AssetDto> _assets;
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
			_productRepo = new ProductRepository(AssetDatabaseHelper.ConnectionString);
			_sut = new AssetRepository(AssetDatabaseHelper.ConnectionString, _productRepo);

			var product = InsertProduct();

			_assets = new List<AssetDto>
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
					Price = 50M,
					PaidBy = "user2",
					Owner = "user2",
					InstallDate = DateTime.Now.AddDays(2),
					InstalledBy = "user2",
					Remark = "To be installed within 2 days",
					TeamAsset = false
				},
				new AssetDto
				{
					CreatedBy = "user3",
					Tag = "CRD-X-00003",
					Product = product,
					PaidBy = "user3",
					Owner = "user3",
					TeamAsset = true
				},
				new AssetDto
				{
					CreatedBy = "user4",
					Tag = "CRD-X-00004",
					Product = product,
					PaidBy = "user4",
					Owner = "user4",
					TeamAsset = true
				},
				new AssetDto
				{
					CreatedBy = "user5",
					Tag = "CRD-X-00005",
					Product = product,
					PaidBy = "user5",
					Owner = "user5",
					TeamAsset = true
				},
			};

			_assets.ForEach(x => x.Id = _sut.Insert(x));
		}

		[TestMethod]
		public void ThenTheResultIsAsExpected()
		{
			var owners = new[] { "user1", "user2", "user3", "user4" };
			var actual = _sut.ForOwners(owners).ToList();

			var expected = _assets.Where(x => owners.Contains(x.Owner)).ToList();

			Assert.AreEqual(4, actual.Count);
			foreach (var expectedDto in expected)
			{
				var actualDto = actual.Single(x => x.Id == expectedDto.Id);
				Assert.AreEqual(expectedDto, actualDto);
			}
		}

		[TestCleanup]
		public void CleanUp()
		{
			_assets.ForEach(x => AssetDatabaseHelper.HardDelete(x.Id.Value));
			_productRepo.Delete(_assets.First().Product.Id.Value);
		}
	}
}
