using LinkIT.Data.Builders;
using LinkIT.Data.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data;

namespace LinkIT.Data.UnitTests.BuilderTests
{
	[TestClass]
	public class WhenBuildingAWhereClauseWithSoftDelete
	{
		private WhereClauseBuilder _sut;
		private Mock<IDbCommand> _mock;

		[TestInitialize]
		public void Setup()
		{
			_mock = new Mock<IDbCommand>();

			_sut = new WhereClauseBuilder(
				_mock.Object,
				LogicalOperator.OR,
				true);
		}

		[TestMethod]
		public void ThenTheWhereClauseIsAsExpected()
		{
			_sut.ForParameter("John Doe", "Name", SqlDbType.VarChar)
				.ForParameter(35, "Age", SqlDbType.Int)
				.ForParameter("Leuven", "Location", SqlDbType.VarChar)
				.ForParameter((string)null, "ShouldBeOmitted", SqlDbType.VarChar);

			string expected = $"WHERE [Deleted] = 0{Environment.NewLine}";
			expected += $"OR{Environment.NewLine}";
			expected += $"[Name] = @Name{Environment.NewLine}";
			expected += $"OR{Environment.NewLine}";
			expected += $"[Age] = @Age{Environment.NewLine}";
			expected += $"OR{Environment.NewLine}";
			expected += $"[Location] = @Location{Environment.NewLine}";

			string actual = _sut.ToString();

			Assert.AreEqual(expected, actual);
		}
	}
}