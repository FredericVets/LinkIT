using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.ProductRepo
{
	[TestClass]
	public class WhenQueryingTheProductsWithOneConditionAndPaging
	{
		private List<ProductDto> _products;
		private ProductRepository _sut;

		[TestInitialize]
		public void Setup()
		{
			_sut = new ProductRepository(new ConnectionString());

			_products = new List<ProductDto>()
			{
				new ProductDto
				{
					CreatedBy = "user1",
					Brand = "HP",
					Type = "EliteBook"
				},
				new ProductDto
				{
					CreatedBy = "user1",
					Brand = "Dell",
					Type = "Latitude 7280"
				},
				new ProductDto
				{
					CreatedBy = "user1",
					Brand = "Dell",
					Type = "Latitude 7390"
				},
				new ProductDto
				{
					CreatedBy = "user1",
					Brand = "Dell",
					Type = "Latitude 7490"
				},
				new ProductDto
				{
					CreatedBy = "user1",
					Brand = "Dell",
					Type = "Latitude 5590"
				},
			};

			_products.ForEach(x => x.Id = _sut.Insert(x));
		}

		[TestMethod]
		public void ThenTheResultIsAsExpected()
		{
			var query = new ProductQuery { CreatedBy = "user1" };
			var pageInfo = new PageInfo(
				2,
				2,
				new OrderBy(ProductRepository.CREATED_BY_COLUMN, Order.DESCENDING));
			var actual = _sut.PagedQuery(pageInfo, query);

			// Simulate the paging on the in-memory collection.
			var expected = _products.OrderByDescending(x => x.CreatedBy).Skip(2).Take(2).ToList();

			Assert.AreEqual(pageInfo, actual.PageInfo);
			Assert.AreEqual(5, actual.TotalCount);
			Assert.AreEqual(2, actual.Result.Count());
			foreach (var item in expected)
			{
				var actualDto = actual.Result.Single(x => x.Id == item.Id);
				Assert.AreEqual(item, actualDto);
			}
		}

		[TestCleanup]
		public void CleanUp()
		{
			new DatabaseHelper().HardDeleteAll();
		}
	}
}