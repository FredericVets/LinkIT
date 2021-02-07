﻿using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Paging;
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
			var owners = new[] { "user1", "user2", "user3", "user4", "user5" };
			var pageInfo = new PageInfo(
				2,
				2,
				new OrderBy(AssetRepository.OWNER_COLUMN, Order.DESCENDING));
			var actual = _sut.ForOwners(pageInfo, owners);

			var expected = _assets
				.Where(x => owners.Contains(x.Owner))
				.OrderByDescending(x => x.Owner)
				.Skip(2)
				.Take(2)
				.ToList();

			Assert.AreEqual(pageInfo, actual.PageInfo);
			Assert.AreEqual(5, actual.TotalCount);
			Assert.AreEqual(2, actual.Result.Count());
			foreach (var expectedDto in expected)
			{
				var actualDto = actual.Result.Single(x => x.Id == expectedDto.Id);
				Assert.AreEqual(expectedDto, actualDto);
			}
		}

		[TestCleanup]
		public void CleanUp() =>
			new DatabaseHelper().HardDeleteAll();
	}
}