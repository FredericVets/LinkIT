using LinkIT.Web.Infrastructure.Api.Shibboleth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;

namespace LinkIT.Web.UnitTests.Infrastructure.Api.ShibbolethServerVariablesTests
{
	[TestClass]
	public class WhenGettingAnAttribute
	{
		private ShibbolethAttributes _sut;

		private NameValueCollection CreateVariables() =>
			new NameValueCollection
			{
				{ "whatever", "whatever" },
				{ ShibbolethAttributes.UID_KEY, "u0000001" },
				{ "bla", "bla" }
			};

		[TestInitialize]
		public void Setup() =>
			_sut = new ShibbolethAttributes(CreateVariables());

		[TestMethod]
		public void ThenTheAttributeIsFound()
		{
			string uid = _sut.Get(ShibbolethAttributes.UID_KEY);
			Assert.AreEqual("u0000001", uid);

			uid = _sut.GetUid();
			Assert.AreEqual("u0000001", uid);

			var result = _sut.GetAll();
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("u0000001", result[ShibbolethAttributes.UID_KEY]);
		}
	}
}