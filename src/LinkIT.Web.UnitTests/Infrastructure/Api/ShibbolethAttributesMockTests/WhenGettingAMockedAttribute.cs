using LinkIT.Data;
using LinkIT.Web.Infrastructure.Api.Shibboleth;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Web.UnitTests.Infrastructure.Api.ShibbolethAttributesMockTests
{
	[TestClass]
	public class WhenGettingAMockedAttribute
	{
		private ShibbolethAttributesMock _sut;

		[TestInitialize]
		public void Setup()
		{
			var kvps = $"{ShibbolethAttributes.UID_KEY}: u0000001, shib_whatever: whatever, shib_x:  y";
			_sut = new ShibbolethAttributesMock(kvps.SplitKeyValuePairs());
		}

		[DataTestMethod]
		[DataRow(ShibbolethAttributes.UID_KEY, "u0000001")]
		[DataRow("shib_whatever", "whatever")]
		[DataRow("shib_x", "y")]
		public void ThenAnExistingKeyIsFound(string key, string expected)
		{
			string actual = _sut.Get(key);
			Assert.AreEqual(expected, actual);
		}
	}
}