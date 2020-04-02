using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.SpecialOwnerRepo
{
	[TestClass]
	public class WhenQueryingTheSpecialOwnersWithNoCondition
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
			var actual = _sut.Query().ToList();

			Assert.AreEqual(_specialOwners.Count, actual.Count);
			foreach (var expectedDto in _specialOwners)
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