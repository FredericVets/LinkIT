using LinkIT.Web.Infrastructure.Api.Shibboleth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Net.Http.Headers;

namespace LinkIT.Web.UnitTests.Infrastructure.Api.ShibbolethHeadersTests
{
	[TestClass]
	public class WhenGettingAValueFromAHeader
	{
		private ShibbolethHeaders _sut;

		private HttpRequestHeaders CreateHeaders()
		{
			var request = new HttpRequestMessage();
			request.Headers.Add("whatever", "whatever");
			request.Headers.Add(ShibbolethVariablesBase.UID_KEY, "u0000001");
			request.Headers.Add(ShibbolethVariablesBase.UID_KEY, "u0000002");
			request.Headers.Add("bla", "bla");

			return request.Headers;
		}

		[TestInitialize]
		public void Setup() =>
			_sut = new ShibbolethHeaders(CreateHeaders());

		[TestMethod]
		public void TheAnExistingHeaderIsFound()
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