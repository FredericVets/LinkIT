using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.AssetRepo
{
	[TestClass]
	public class WhenQueryingTheAssetsWithNoCondition
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
					Price = 50M,
					PaidBy = "user2",
					Owner = "user1",
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
					Owner = "user1",
					TeamAsset = true
				},
				new AssetDto
				{
					CreatedBy = "user4",
					Tag = "CRD-X-00004",
					Product = product,
					PaidBy = "user4",
					Owner = "user1",
					TeamAsset = true
				},
				new AssetDto
				{
					CreatedBy = "user5",
					Tag = "CRD-X-00005",
					Product = product,
					PaidBy = "user5",
					Owner = "user1",
					TeamAsset = true
				},
			};

			_expected.ForEach(x => x.Id = _sut.Insert(x));
		}

		[TestMethod]
		public void ThenTheResultIsAsExpected()
		{
			var actual = _sut.Query().ToList();

			Assert.AreEqual(5, actual.Count);
			foreach (var expectedDto in _expected)
			{
				var actualDto = actual.Single(x => x.Id == expectedDto.Id);
				Assert.AreEqual(expectedDto, actualDto);
			}
		}

		[TestCleanup]
		public void CleanUp()
		{
			new DatabaseHelper().HardDeleteAll();
		}
	}
}