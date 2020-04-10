using LinkIT.Web.Infrastructure.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Specialized;

namespace LinkIT.Web.UnitTests.Infrastructure.Api.ShibbolethAttributesTests
{
	[TestClass]
	public class WhenGettingAnAttributeThatDoesntExist
	{
		private ShibbolethAttributes _sut;

		private NameValueCollection CreateValues() =>
			new NameValueCollection()
			{
				{ "whatever", "whatever" },
			};

		[TestInitialize]
		public void Setup()
		{
			var mock = MockFactory.Create(CreateValues);

			_sut = new ShibbolethAttributes(mock.Object);
		}

		[TestMethod]
		public void ThenTheAttributeIsNotFound()
		{
			bool result = _sut.TryGet("iDontExist", out _);
			Assert.IsFalse(result);

			Assert.ThrowsException<ArgumentException>(() => _sut.Get("iDontExist"));
		}
	}
}