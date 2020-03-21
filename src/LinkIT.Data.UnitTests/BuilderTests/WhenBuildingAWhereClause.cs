﻿using LinkIT.Data.Builders;
using LinkIT.Data.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data;

namespace LinkIT.Data.UnitTests.BuilderTests
{
	[TestClass]
	public class WhenBuildingAWhereClause
	{
		private WhereClauseBuilder _sut;
		private Mock<IDbCommand> _mock;

		[TestInitialize]
		public void Setup()
		{
			_mock = new Mock<IDbCommand>();

			_sut = new WhereClauseBuilder(
				_mock.Object,
				LogicalOperator.AND,
				false);
		}

		[TestMethod]
		public void ThenTheWhereClauseIsAsExpected()
		{
			_sut.AddParameter("John Doe", "Name", SqlDbType.VarChar);
			_sut.AddParameter(35, "Age", SqlDbType.Int);
			_sut.AddParameter("Leuven", "Location", SqlDbType.VarChar);
			_sut.AddParameter((string)null, "ShouldBeOmitted", SqlDbType.VarChar);

			string expected = $"WHERE{Environment.NewLine}";
			expected += $"[Name] = @Name{Environment.NewLine}";
			expected += $"AND{Environment.NewLine}";
			expected += $"[Age] = @Age{Environment.NewLine}";
			expected += $"AND{Environment.NewLine}";
			expected += $"[Location] = @Location{Environment.NewLine}";

			string actual = _sut.ToString();

			Assert.AreEqual(expected, actual);
		}
	}
}