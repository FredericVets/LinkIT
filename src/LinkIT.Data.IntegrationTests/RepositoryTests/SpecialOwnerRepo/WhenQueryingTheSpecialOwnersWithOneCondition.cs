using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.SpecialOwnerRepo
{
	[TestClass]
	public class WhenQueryingTheSpecialOwnersWithOneCondition
	{
		private List<SpecialOwnerDto> _specialOwners;
		private SpecialOwnerRepository _sut;

		[TestInitialize]
		public void Setup()
		{
			_sut = new SpecialOwnerRepository(new ConnectionString());

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
					Name = "special1"
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
			var actual = _sut.Query(new SpecialOwnerQuery { Name = "special1" });

			Assert.AreEqual(2, actual.Count());

			foreach (var actualDto in actual)
			{
				var expectedDto = _specialOwners.Single(x => x.Id == actualDto.Id);
				Assert.AreEqual(expectedDto, actualDto);
			}
		}

		[TestCleanup]
		public void CleanUp() =>
			new DatabaseHelper().HardDeleteAll();
	}
}