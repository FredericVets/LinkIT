using System;
using System.Collections.Generic;
using System.Linq;
using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Paging;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.SpecialOwnerRepo
{
	[TestClass]
	public class WhenQueryingTheSpecialOwnersWithNoConditionAndPaging
	{
		private List<SpecialOwnerDto> _specialOwners;
		private SpecialOwnerRepository _sut;

		[TestInitialize]
		public void Setup()
		{
			_sut = new SpecialOwnerRepository(ConnectionString.Get());

			_specialOwners = new List<SpecialOwnerDto>()
			{
				new SpecialOwnerDto
				{
					CreatedBy = "user1",
					Name = "Special 1"
				},
				new SpecialOwnerDto
				{
					CreatedBy = "user1",
					Name = "Special 2",
					Remark = "Remark 2"
				},
				new SpecialOwnerDto
				{
					CreatedBy = "user1",
					Name = "Special 3",
					Remark = "Remark 3"
				},
				new SpecialOwnerDto
				{
					CreatedBy = "user1",
					Name = "Special 4"
				},
				new SpecialOwnerDto
				{
					CreatedBy = "user1",
					Name = "Special 5"
				},

			};

			_specialOwners.ForEach(x => x.Id = _sut.Insert(x));
		}

		[TestMethod]
		public void ThenTheResultIsAsExpected()
		{
			var pageInfo = new PageInfo(
				2,
				2,
				new OrderBy(SpecialOwnerRepository.CREATED_BY_COLUMN, Order.DESCENDING));
			var actual = _sut.PagedQuery(pageInfo);

			// Simulate the paging on the in-memory collection.
			var expected = _specialOwners.OrderByDescending(x => x.CreatedBy).Skip(2).Take(2).ToList();

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
		public void CleanUp() =>
			new DatabaseHelper().HardDeleteAll();
	}
}