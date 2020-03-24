using LinkIT.Data.Builders;
using LinkIT.Data.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data;

namespace LinkIT.Data.UnitTests.BuilderTests
{
	[TestClass]
	public class WhenBuildingAWhereClauseWithNoArgumentsAndSoftDelete
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
				true);
		}

		[TestMethod]
		public void ThenTheWhereClauseIsAsExpected()
		{
			_sut.ForParameter<int?>(null, "Whatever", SqlDbType.VarChar);

			string expected = $"WHERE{Environment.NewLine}";
			expected += $"[Deleted] = 0{Environment.NewLine}";

			string actual = _sut.ToString();

			Assert.AreEqual(expected, actual);
		}
	}
}