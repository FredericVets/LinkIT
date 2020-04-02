using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.SpecialOwnerRepo
{
	[TestClass]
	public class WhenUpdatingASpecialOwner
	{
		private List<SpecialOwnerDto> _expected;
		private SpecialOwnerRepository _sut;

		private void AssertDto(SpecialOwnerDto expected)
		{			
			var actual = _sut.GetById(expected.Id.Value);

			Assert.IsNotNull(actual);
			Assert.AreEqual(expected, actual);
		}

		[TestInitialize]
		public void Setup()
		{
			_sut = new SpecialOwnerRepository(ConnectionString.Get());

			_expected = new List<SpecialOwnerDto>
			{
				new SpecialOwnerDto
				{
					CreatedBy = "user1",
					Name = "Special one",
					Remark = "I'm special"
				},
				new SpecialOwnerDto
				{
					CreatedBy = "user2",
					Name = "The default one"
					// Don't set the remark
				}
			};

			_expected.ForEach(x => x.Id = _sut.Insert(x));
		}

		[TestMethod]
		public void ThenTheDataIsUpdated()
		{
			foreach (var owner in _expected)
			{
				owner.Name = "another name";
				owner.ModifiedBy = "user2";
			}

			_sut.Update(_expected);

			foreach (var owner in _expected)
				AssertDto(owner);
		}

		[TestCleanup]
		public void Cleanup() =>
			new DatabaseHelper().HardDeleteAll();
	}
}