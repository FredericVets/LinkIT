using System;
using System.Collections.Generic;
using System.Linq;
using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.SpecialOwnerRepo
{
	[TestClass]
	public class WhenQueryingTheSpecialOwnersWithOneConditionWithDateTime
	{
		private List<SpecialOwnerDto> _specialOwners;
		private SpecialOwnerRepository _sut;

		[TestInitialize]
		public void Setup()
		{
			_sut = new SpecialOwnerRepository(new ConnectionString());

			DateTimeProvider.SetDateTime(new DateTime(2020, 04, 25, 23, 30, 00));

			_specialOwners = new List<SpecialOwnerDto>()
			{
				new SpecialOwnerDto
				{
					CreatedBy = "user1",
					Name = "special1"
				},
				new SpecialOwnerDto
				{
					CreatedBy = "user1",
					Name = "special2",
					Remark = "remark2"
				},
				new SpecialOwnerDto
				{
					CreatedBy = "user1",
					Name = "special3",
					Remark = "remark3"
				},
				new SpecialOwnerDto
				{
					CreatedBy = "user1",
					Name = "special4"
				},
				new SpecialOwnerDto
				{
					CreatedBy = "user1",
					Name = "special5"
				},

			};

			_specialOwners.ForEach(x => x.Id = _sut.Insert(x));
		}

		[TestMethod]
		public void ThenTheResultIsAsExpected()
		{
			var actual = _sut.Query(new SpecialOwnerQuery { CreationDate = new DateTime(2020, 04, 25) });

			Assert.AreEqual(5, actual.Count());

			foreach (var actualDto in actual)
			{
				var expectedDto = _specialOwners.Single(x => x.Id == actualDto.Id);
				Assert.AreEqual(expectedDto, actualDto);
			}
		}

		[TestCleanup]
		public void CleanUp()
		{
			DateTimeProvider.ResetDateTime();
			new DatabaseHelper().HardDeleteAll();
		}
			
	}
}