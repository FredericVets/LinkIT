using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.AssetHistoryRepo
{
	[TestClass]
	public class WhenQueryingTheAssetHistoriesWithNoCondition
	{
		private ProductRepository _productRepo;
		private AssetRepository _assetRepo;
		private AssetHistoryRepository _sut;

		private IList<AssetDto> _expectedHistory;
		private IList<AssetDto> _assets;

		private static void AssertAssetHistory(
			IList<AssetDto> assets,
			IList<AssetHistoryDto> actualHistory,
			IList<AssetDto> expectedHistory)
		{
			foreach (var asset in assets)
				AssertAssetHistory(
					asset.Id.Value,
					actualHistory,
					expectedHistory);
		}

		private static void AssertAssetHistory(
			long assetId,
			IList<AssetHistoryDto> actualHistory,
			IList<AssetDto> expectedHistory)
		{
			var subsetActual = actualHistory
				.Where(x => x.AssetId == assetId)
				.OrderBy(x => x.ModificationDate)
				.ToList();
			var subsetExpected = expectedHistory
				.Where(x => x.Id == assetId)
				.OrderBy(x => x.ModificationDate)
				.ToList();

			Assert.AreEqual(subsetExpected.Count, subsetActual.Count);
			for (int i = 0; i < subsetActual.Count; i++)
			{
				var actual = subsetActual[i];
				var expected = subsetExpected[i];

				var comparer = new AssetDtoForHistoryEqualityComparer();
				Assert.IsTrue(comparer.Equals(expected, actual));
			}
		}

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

		private void AddToHistory(IEnumerable<AssetDto> input)
		{
			foreach (var item in input)
			{
				var clone = item.ShallowCopy();
				_expectedHistory.Add(clone);
			}
		}

		private void UpdateAssets(IList<AssetDto> input)
		{
			// First modification for all assets.
			input[0].Tag = "CRD-X-00004";
			input[0].ModifiedBy = "userX";

			input[1].Description = "Modified description";
			input[1].Price = 11.12M;
			input[1].ModifiedBy = "userX";

			input[2].Price = 12.34M;
			input[2].InstallDate = DateTime.Now.AddDays(3);
			input[2].PaidBy = "userX";
			input[2].ModifiedBy = "userX";

			_assetRepo.Update(input);
			AddToHistory(input);

			// And a second modification for one asset;
			input[1].Description = "Yet another modified description";
			input[1].ModifiedBy = "userY";

			_assetRepo.Update(input[1]);
			AddToHistory(new[] { input[1] });
		}

		[TestInitialize]
		public void Setup()
		{
			var connString = new ConnectionString();
			_productRepo = new ProductRepository(connString);
			_assetRepo = new AssetRepository(connString, _productRepo);
			_sut = new AssetHistoryRepository(connString);

			_expectedHistory = new List<AssetDto>();

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
					Price = 50.01M,
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
			};

			_assets.ToList().ForEach(x => x.Id = _assetRepo.Insert(x));
			AddToHistory(_assets);

			UpdateAssets(_assets);
		}

		[TestMethod]
		public void ThenTheHistoryIsAsExpected()
		{
			var actualHistory = _sut.Query().ToList();

			// In total there should be 7 AssetHistoryDto items.
			// For the 3 inserts and all of the subsequent updates(4).
			Assert.AreEqual(7, actualHistory.Count);

			AssertAssetHistory(
				_assets,
				actualHistory,
				_expectedHistory);
		}

		[TestCleanup]
		public void CleanUp() =>
			new DatabaseHelper().HardDeleteAll();
	}
}