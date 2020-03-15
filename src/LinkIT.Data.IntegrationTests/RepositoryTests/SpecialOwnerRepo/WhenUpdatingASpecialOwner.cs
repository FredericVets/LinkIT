using LinkIT.Data.DTO;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.SpecialOwnerRepo
{
	[TestClass]
	public class WhenUpdatingASpecialOwner
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
			var conStr = ConfigurationManager.ConnectionStrings["LinkITConnectionString"].ConnectionString;
			_sut = new SpecialOwnerRepository(conStr);

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

			foreach (var owner in _expected)
				owner.Id = _sut.Insert(owner);
		}

		[TestMethod]
		public void ThenTheDataIsInserted()
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
		public void Cleanup()
		{
			_expected.ToList().ForEach(x => _sut.Delete(x.Id.Value));
		}
	}
}