using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace LinkIT.Data.DTO.UnitTests
{
	[TestClass]
	public class WhenUsingTheUserRolesDto
	{
		private UserRolesDto _sut;

		[TestInitialize]
		public void Setup()
		{
			var data = new Dictionary<string, IEnumerable<string>>
			{
				["user1"] = new[] { "select", "create" },
				["user2"] = new[] { "select" },
				["user3"] = null
			};

			_sut = new UserRolesDto(data);
		}

		[DataTestMethod]
		[DataRow("user1")]
		[DataRow("user2")]
		[DataRow("user3")]
		public void ThenTheUsersAreRetrievedCorrectly(string user) =>
			Assert.IsTrue(_sut.HasUser(user));

		[DataTestMethod]
		[DataRow("user1", new object[] { new[] { "select" } })]
		[DataRow("user1", new object[] { new[] { "select", "create" } })]
		[DataRow("user2", new object[] { new[] { "select" } })]
		public void ThenTheUserRolesAreRetrievedCorrectly(string user, params string[] roles) =>
			Assert.IsTrue(_sut.HasRole(user, roles));

		[DataTestMethod]
		[DataRow("")]
		[DataRow(null)]
		[DataRow("idontexist")]
		public void ThenNonExistentUsersAreHandledCorrectly(string user) =>
			Assert.IsFalse(_sut.HasUser(user));

		[DataTestMethod]
		[DataRow("user1", "idontexist")]
		[DataRow("user1", "")]
		[DataRow("user3", "idontexist")]
		public void ThenNonExistentUserRolesAreHandledCorrectly(string user, string role) =>
			Assert.IsFalse(_sut.HasRole(user, role));

		[TestMethod]
		public void ThenExpectedExceptionsAreThrown()
		{
			Assert.ThrowsException<ArgumentException>(
				() => _sut.HasRole("idontexist", string.Empty),
				"User 'idontexist' not found.");

			Assert.ThrowsException<ArgumentException>(
				() => _sut.GetRolesFor("user3"),
				"No roles found for user 'user3'.");
		}
	}
}