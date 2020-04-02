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

		[TestMethod]
		public void ThenExistentDataIsRetrievedCorrectly()
		{
			Assert.IsTrue(_sut.HasUser("user1"));
			Assert.IsTrue(_sut.HasRole("user1", "select"));
			Assert.IsTrue(_sut.HasRole("user1", "create"));

			Assert.IsTrue(_sut.HasUser("user2"));
			Assert.IsTrue(_sut.HasRole("user2", "select"));

			Assert.IsTrue(_sut.HasUser("user3"));
		}

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