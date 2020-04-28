using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.SpecialOwnerRepo
{
	[TestClass]
	public class WhenQueryingTheSpecialOwnersWithOneConditionWithDateRange
	{
		private List<SpecialOwnerDto> _specialOwners;
		private SpecialOwnerRepository _sut;

		private static void CreateAndInsertData(SpecialOwnerRepository repo, List<SpecialOwnerDto> specialOwners)
		{
			var timeStamp = new DateTime(2020, 04, 25, 23, 30, 00);

			for (int i = 0; i < 5; i++)
			{
				DateTimeProvider.SetDateTime(timeStamp);

				var owner = new SpecialOwnerDto
				{
					CreatedBy = "user" + i,
					Name = "special" + i
				};

				owner.Id = repo.Insert(owner);
				specialOwners.Add(owner);

				timeStamp = timeStamp.AddDays(1);
			}
		}

		private static void AssertResult(
			IEnumerable<SpecialOwnerDto> expected, 
			int expectedCount, 
			IEnumerable<SpecialOwnerDto> actual)
		{
			Assert.AreEqual(expectedCount, actual.Count());
			foreach (var actualDto in actual)
			{
				var expectedDto = expected.Single(x => x.Id == actualDto.Id);
				Assert.AreEqual(expectedDto, actualDto);
			}
		}

		[TestInitialize]
		public void Setup()
		{
			_sut = new SpecialOwnerRepository(new ConnectionString());
			_specialOwners = new List<SpecialOwnerDto>();

			CreateAndInsertData(_sut, _specialOwners);
		}

		[TestMethod]
		public void ThenTheResultIsAsExpectedForARangeWithOnlyStartDate()
		{
			var query = new SpecialOwnerQuery
			{
				CreationDateRange = new DateRange(new DateTime(2020, 04, 25), null)
			};

			var actual = _sut.Query(query);
			AssertResult(_specialOwners, 5, actual);
		}

		[TestMethod]
		public void ThenTheResultIsAsExpectedForARangeWithOnlyEndDate()
		{
			var query = new SpecialOwnerQuery
			{
				CreationDateRange = new DateRange(null, new DateTime(2020, 04, 29))
			};
			
			var actual = _sut.Query(query);
			AssertResult(_specialOwners, 5, actual);
		}

		[TestMethod]
		public void ThenTheResultIsAsExpectedForARangeWithBothStartAndEndDate()
		{
			var query = new SpecialOwnerQuery
			{
				CreationDateRange = new DateRange(new DateTime(2020, 04, 26), new DateTime(2020, 04, 28))
			};
			
			var actual = _sut.Query(query);
			AssertResult(_specialOwners, 3, actual);
		}

		[TestCleanup]
		public void CleanUp()
		{
			DateTimeProvider.ResetDateTime();
			new DatabaseHelper().HardDeleteAll();
		}
	}
}