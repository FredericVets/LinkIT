using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.ProductRepo
{
	[TestClass]
	public class WhenQueryingTheProductsWithNoCondition
	{
		private List<ProductDto> _expected;
		private ProductRepository _sut;

		[TestInitialize]
		public void Setup()
		{
			_sut = new ProductRepository(new ConnectionString());

			_expected = new List<ProductDto>()
			{
				new ProductDto
				{
					CreatedBy = "user1",
					Brand = "HP",
					Type = "EliteBook",
					Group = "Laptop"
				},
				new ProductDto
				{
					CreatedBy = "user1",
					Brand = "Dell",
					Type = "Latitude 7280",
					Group = "Laptop"
				},
				new ProductDto
				{
					CreatedBy = "user1",
					Brand = "Dell",
					Type = "Latitude 7390",
					Group = "Laptop"
				},
				new ProductDto
				{
					CreatedBy = "user1",
					Brand = "Dell",
					Type = "Latitude 7490",
					Group = "Laptop"
				},
				new ProductDto
				{
					CreatedBy = "user1",
					Brand = "Dell",
					Type = "Latitude 5590",
					Group = "Laptop"
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
		public void CleanUp() => 
			new DatabaseHelper().HardDeleteAll();
	}
}