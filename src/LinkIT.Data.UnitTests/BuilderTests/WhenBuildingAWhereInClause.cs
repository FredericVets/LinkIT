using LinkIT.Data.DTO;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data;
using System.Data.SqlClient;

namespace LinkIT.Data.UnitTests.BuilderTests
{
	//[TestClass]

	// SqlParameterCollection is not mockable ....
	// Possible workaround = https://stackoverflow.com/questions/6376715/how-to-mock-sqlparametercollection-using-moq
	public class WhenBuildingAWhereInClause
	{
		private WhereInClauseBuilder _sut;
		private Mock<SqlParameterCollection> _paramCollectionMock = new Mock<SqlParameterCollection>();

		//[TestInitialize]
		public void Setup()
		{
			_sut = new WhereInClauseBuilder(
				Repository<Dto, Query>.ID_COLUMN,
				_paramCollectionMock.Object,
				true);
		}

		//[TestMethod]
		public void ThenTheWhereInClauseIsAsExpected()
		{
			var ids = new[] { 1, 2, 3, 4, 5 };
			_sut.AddParameters(ids, SqlDbType.BigInt);

			string expected = "WHERE ID IN (1, 2, 3, 4, 5)";
			expected += "AND DELETED = 0";

			string actual = _sut.ToString();

			Assert.AreEqual(expected, actual);
		}
	}
}