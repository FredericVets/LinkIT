﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using LinkIT.Data.DTO;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
			var conStr = ConfigurationManager.ConnectionStrings["LinkITConnectionString"].ConnectionString;
			_sut = new ProductRepository(conStr);

			_expected = new List<ProductDto>()
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
			_expected.ForEach(x => _sut.Delete(x.Id.Value));
		}
	}
}