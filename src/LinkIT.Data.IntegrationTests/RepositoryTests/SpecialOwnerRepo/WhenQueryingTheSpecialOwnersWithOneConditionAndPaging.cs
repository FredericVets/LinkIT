using LinkIT.Data.DTO;
using LinkIT.Data.Paging;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.SpecialOwnerRepo
{
	[TestClass]
	public class WhenQueryingTheSpecialOwnersWithOneConditionAndPaging
	{
		private List<SpecialOwnerDto> _expected;
		private SpecialOwnerRepository _sut;

		[TestInitialize]
		public void Setup()
		{
			var conStr = ConfigurationManager.ConnectionStrings["LinkITConnectionString"].ConnectionString;
			_sut = new SpecialOwnerRepository(conStr);

			_expected = new List<SpecialOwnerDto>()
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

			foreach (var owner in _expected)
				owner.Id = _sut.Insert(owner);
		}

		[TestMethod]
		public void ThenTheResultIsAsExpected()
		{
			var query = new SpecialOwnerQuery { CreatedBy = "user1" };
			var pageInfo = new PageInfo(
				2,
				2,
				new OrderBy(Repository.CREATED_BY_COLUMN, Order.DESCENDING));
			var actual = _sut.PagedQuery(pageInfo, query);

			// Simulate the paging on the in-memory collection.
			var page = _expected.OrderByDescending(x => x.CreatedBy).Skip(2).Take(2).ToList();

			Assert.AreEqual(pageInfo, actual.PageInfo);
			Assert.AreEqual(5, actual.TotalCount);
			Assert.AreEqual(2, actual.Result.Count());
			foreach (var item in page)
			{
				var actualDto = actual.Result.Single(x => x.Id == item.Id);
				Assert.AreEqual(item, actualDto);
			}
		}

		[TestCleanup]
		public void CleanUp()
		{
			_expected.ForEach(x => _sut.Delete(x.Id.Value));
		}
	}
}