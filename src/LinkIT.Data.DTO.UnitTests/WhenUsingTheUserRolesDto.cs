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
		public void ThenTheUsersAreRetrievedCorrectly(string userName) =>
			Assert.IsTrue(_sut.HasUser(userName));

		[DataTestMethod]
		[DataRow("user1", "select")]
		[DataRow("user1", "create")]
		[DataRow("user2", "select")]
		public void ThenTheUserRolesAreRetrievedCorrectly(string user, string role) =>
			Assert.IsTrue(_sut.HasRole(user, role));

		[TestMethod]
		public void ThenNonExistentDataIsHandledCorrectly()
		{
			Assert.IsFalse(_sut.HasUser("idontexist"));

			Assert.IsFalse(_sut.HasRole("user1", "idontexist"));

			Assert.IsFalse(_sut.HasRole("user3", "idontexist"));
		}

		[TestMethod]
		public void ThenExpectedExceptionsAreThrown()
		{
			Assert.ThrowsException<ArgumentNullException>(
				() => _sut.HasUser(string.Empty),
				"user");

			Assert.ThrowsException<ArgumentException>(
				() => _sut.HasRole("idontexist", string.Empty),
				"User 'idontexist' not found.");

			Assert.ThrowsException<ArgumentNullException>(
				() => _sut.HasRole("user1", string.Empty),
				"user");

			Assert.ThrowsException<ArgumentException>(
				() => _sut.GetRolesFor("user3"),
				"No roles found for user 'user3'.");
		}
	}
}