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
	[TestClass]
	public class WhenBuildingAWhereInClause
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
				false);
		}

		[TestMethod]
		public void ThenTheWhereInClauseIsAsExpected()
		{
			var ids = new[] { 1, 2, 3, 4, 5 };
			_sut.ForParameters(ids, SqlDbType.BigInt);

			string expected = $"WHERE [Id] IN (@Value0, @Value1, @Value2, @Value3, @Value4){Environment.NewLine}";
			string actual = _sut.ToString();

			Assert.AreEqual(expected, actual);
		}
	}
}