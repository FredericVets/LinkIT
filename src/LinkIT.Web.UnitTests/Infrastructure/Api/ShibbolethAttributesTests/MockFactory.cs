using Moq;
using System;
using System.Collections.Specialized;
using System.Web;

namespace LinkIT.Web.UnitTests.Infrastructure.Api.ShibbolethAttributesTests
{
	internal static class MockFactory
	{
		internal static Mock<HttpContextBase> Create(Func<NameValueCollection> valuesFunc)
		{
			var ctxMock = new Mock<HttpContextBase>(MockBehavior.Strict);
			var reqMock = new Mock<HttpRequestBase>(MockBehavior.Strict);

			reqMock.Setup(x => x.ServerVariables).Returns(valuesFunc);
			ctxMock.Setup(x => x.Request).Returns(reqMock.Object);

			return ctxMock;
		}
	}
}