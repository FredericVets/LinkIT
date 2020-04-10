using LinkIT.Web.Infrastructure.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace LinkIT.Web.UnitTests.Infrastructure.Api.ShibbolethHeadersTests
{
	[TestClass]
	public class WhenGettingAValueFromAHeaderThatDoesntExist
	{
		private ShibbolethHeaders _sut;

		private HttpRequestHeaders CreateHeaders()
		{
			var request = new HttpRequestMessage();
			request.Headers.Add("whatever", "whatever");

			return request.Headers;
		}

		[TestInitialize]
		public void Setup() =>
			_sut = new ShibbolethHeaders(CreateHeaders());

		[TestMethod]
		public void TheAnExistingHeaderIsFound()
		{
			bool result = _sut.TryGet("iDontExist", out _);
			Assert.IsFalse(result);

			result = _sut.TryGet("SHIB_iDontExist", out _);
			Assert.IsFalse(result);

			Assert.ThrowsException<ArgumentException>(() => _sut.Get("iDontExist"));
		}
	}
}