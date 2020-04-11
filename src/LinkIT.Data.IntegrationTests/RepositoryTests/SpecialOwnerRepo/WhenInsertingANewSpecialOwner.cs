using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.SpecialOwnerRepo
{
	[TestClass]
	public class WhenInsertingANewSpecialOwner
	{
		private IEnumerable<SpecialOwnerDto> _expected;
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
			_sut = new SpecialOwnerRepository(new ConnectionString());

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
		}

		[TestMethod]
		public void ThenTheDataIsInserted()
		{
			_expected.ToList().ForEach(x => x.Id = _sut.Insert(x));

			foreach (var dto in _expected)
				AssertDto(dto);
		}

		[TestCleanup]
		public void Cleanup() =>
			new DatabaseHelper().HardDeleteAll();
	}
}