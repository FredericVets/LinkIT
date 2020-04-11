using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.SpecialOwnerRepo
{
	[TestClass]
	public class WhenGettingByListOfIds
	{
		private List<SpecialOwnerDto> _expected;
		private SpecialOwnerRepository _sut;

		[TestInitialize]
		public void Setup()
		{
			_sut = new SpecialOwnerRepository(new ConnectionString());

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

			_expected.ForEach(x => x.Id = _sut.Insert(x));
		}

		[TestMethod]
		public void ThenTheExpectedInstancesAreRetrieved()
		{
			var ids = _expected.Select(x => x.Id.Value);
			var actual = _sut.GetById(ids).ToList();

			Assert.AreEqual(_expected.Count, actual.Count);
			foreach (var item in _expected)
			{
				var actualDto = actual.Single(x => x.Id == item.Id);
				Assert.AreEqual(item, actualDto);
			}
		}

		[TestCleanup]
		public void CleanUp() =>
			new DatabaseHelper().HardDeleteAll();
	}
}