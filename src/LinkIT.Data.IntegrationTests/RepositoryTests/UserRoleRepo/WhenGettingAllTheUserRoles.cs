using LinkIT.Data.IntegrationTests.RepositoryTests.Helpers;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.UserRoleRepo
{
	[TestClass]
	public class WhenGettingAllTheUserRoles
	{
		private UserRoleRepository _sut;

		[TestInitialize]
		public void Setup()
		{
			new DatabaseHelper().InsertTestUserRoles();
			_sut = new UserRoleRepository(ConnectionString.Get());
		}

		[TestMethod]
		public void ThenTheRetrievedDataIsAsExpected()
		{
			var userRoles = _sut.GetAll();

			// Assert the data for user1.
			var roles = userRoles.GetRolesFor("user1").ToArray();
			Assert.AreEqual(4, roles.Length);
			Assert.AreEqual("select", roles[0]);
			Assert.AreEqual("create", roles[1]);
			Assert.AreEqual("update", roles[2]);
			Assert.AreEqual("delete", roles[3]);

			// Assert the data for user2.
			roles = userRoles.GetRolesFor("user2").ToArray();
			Assert.AreEqual(2, roles.Length);
			Assert.AreEqual("select", roles[0]);
			Assert.AreEqual("update", roles[1]);

			// Assert the data for user3.
			roles = userRoles.GetRolesFor("user3").ToArray();
			Assert.AreEqual(1, roles.Length);
			Assert.AreEqual("select", roles.Single());
		}

		[TestCleanup]
		public void Cleanup() =>
			new DatabaseHelper().HardDeleteAll();
	}
}