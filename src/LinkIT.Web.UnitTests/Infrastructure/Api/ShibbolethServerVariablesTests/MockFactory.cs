using Moq;
using System;
using System.Collections.Specialized;
using System.Web;

namespace LinkIT.Web.UnitTests.Infrastructure.Api.ShibbolethServerVariablesTests
{
	internal static class MockFactory
	{
		internal static Mock<HttpRequestBase> Create(Func<NameValueCollection> variablesFunc)
		{
			var reqMock = new Mock<HttpRequestBase>(MockBehavior.Strict);
			reqMock.Setup(x => x.ServerVariables).Returns(variablesFunc);

			return reqMock;
		}
	}
}