using LinkIT.Data.DTO;
using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.UserRoleRepo
{
	[TestClass]
	public class WhenGettingAllTheUserRoles
	{
		private UserRolesDto _actual;

		[TestInitialize]
		public void Setup()
		{
			new DatabaseHelper().InsertTestUserRoles();
			var sut = new UserRoleRepository(ConnectionString.Get());

			_actual = sut.GetAll();
		}

		[DataTestMethod]
		[DataRow("user1", "select")]
		[DataRow("user1", "create")]
		[DataRow("user1", "update")]
		[DataRow("user1", "delete")]
		[DataRow("user2", "select")]
		[DataRow("user2", "update")]
		[DataRow("user3", "select")]
		public void ThenTheRetrievedDataIsAsExpected(string userName, string role)
		{
			// Assert the data for user1.
			var roles = _actual.GetRolesFor(userName).ToArray();
			Assert.IsTrue(roles.Contains(role));
		}

		[TestCleanup]
		public void Cleanup() =>
			new DatabaseHelper().HardDeleteAll();
	}
}