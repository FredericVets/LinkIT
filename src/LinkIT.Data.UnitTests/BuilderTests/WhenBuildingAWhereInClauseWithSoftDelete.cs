using LinkIT.Data.Builders;
using LinkIT.Data.DTO;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data;

namespace LinkIT.Data.UnitTests.BuilderTests
{
	// SqlParameterCollection is not mockable ....
	// Possible workaround = https://stackoverflow.com/questions/6376715/how-to-mock-sqlparametercollection-using-moq
	[TestClass]
	public class WhenBuildingAWhereInClauseWithSoftDelete
	{
		private WhereInClauseBuilder _sut;
		private Mock<IDbCommand> _mock;

		[TestInitialize]
		public void Setup()
		{
			_mock = new Mock<IDbCommand>();

			_sut = new WhereInClauseBuilder(
				Repository<Dto, Query>.ID_COLUMN,
				_mock.Object,
				true);
		}

		[TestMethod]
		public void ThenTheWhereInClauseIsAsExpected()
		{
			var ids = new[] { 1, 2, 3, 4, 5 };
			_sut.AddParameters(ids, SqlDbType.BigInt);

			string expected = $"WHERE [Deleted] = 0{Environment.NewLine}";
			expected += $"AND [Id] IN (@Value0, @Value1, @Value2, @Value3, @Value4){Environment.NewLine}";

			string actual = _sut.ToString();

			Assert.AreEqual(expected, actual);
		}
	}
}