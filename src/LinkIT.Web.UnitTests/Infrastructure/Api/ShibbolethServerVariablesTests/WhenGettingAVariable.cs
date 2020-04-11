using LinkIT.Web.Infrastructure.Api.Shibboleth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;

namespace LinkIT.Web.UnitTests.Infrastructure.Api.ShibbolethServerVariablesTests
{
	[TestClass]
	public class WhenGettingAVariable
	{
		private ShibbolethServerVariables _sut;

		private NameValueCollection CreateVariables() =>
			new NameValueCollection()
			{
				{ "whatever", "whatever" },
				{ ShibbolethVariablesBase.UID_KEY, "u0000001" },
				{ "bla", "bla" }
			};

		[TestInitialize]
		public void Setup()
		{
			var mock = MockFactory.Create(CreateVariables);

			_sut = new ShibbolethServerVariables(mock.Object);
		}

		[TestMethod]
		public void ThenAnExistingVariableIsFound()
		{
			string uid = _sut.Get(ShibbolethVariablesBase.UID_KEY);
			Assert.AreEqual("u0000001", uid);

			uid = _sut.GetUid();
			Assert.AreEqual("u0000001", uid);

			var result = _sut.GetAll();
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("u0000001", result[ShibbolethVariablesBase.UID_KEY]);
		}
	}
}