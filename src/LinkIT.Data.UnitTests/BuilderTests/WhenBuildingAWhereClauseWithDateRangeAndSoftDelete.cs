using System;
using System.Data;
using LinkIT.Data.Builders;
using LinkIT.Data.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LinkIT.Data.UnitTests.BuilderTests
{
	[TestClass]
	public class WhenBuildingAWhereClauseWithDateRangeAndSoftDelete
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
			_sut.ForParameter("John Doe", "Name", SqlDbType.VarChar)
				.ForParameter(35, "Age", SqlDbType.Int)
				.ForDateRange(new DateRange(DateTime.Now, null), "CreationDate", SqlDbType.DateTime2)
				.ForDateRange(new DateRange(DateTime.Now, DateTime.Now.AddDays(3)), "ModificationDate", SqlDbType.DateTime2)
				.ForDateRange(new DateRange(null, DateTime.Now.AddDays(3)), "InstallDate", SqlDbType.DateTime2);

			string expected = $"WHERE ({Environment.NewLine}";
			expected += $"[Name] like @Name{Environment.NewLine}";
			expected += $"AND{Environment.NewLine}";
			expected += $"[Age] = @Age{Environment.NewLine}";
			expected += $"AND{Environment.NewLine}";
			expected += $"([CreationDate] >= @CreationDateStart){Environment.NewLine}";
			expected += $"AND{Environment.NewLine}";
			expected += $"([ModificationDate] >= @ModificationDateStart AND [ModificationDate] < @ModificationDateEnd){Environment.NewLine}";
			expected += $"AND{Environment.NewLine}";
			expected += $"([InstallDate] < @InstallDateEnd){Environment.NewLine}";
			expected += $"){Environment.NewLine}";
			expected += $"AND{Environment.NewLine}";
			expected += $"[Deleted] = 0{Environment.NewLine}";

			string actual = _sut.ToString();

			Assert.AreEqual(expected, actual);
		}
	}
}