using LinkIT.Web.Infrastructure.Shibboleth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Specialized;

namespace LinkIT.Web.UnitTests.Infrastructure.ShibbolethAttributesTests
{
	[TestClass]
	public class WhenGettingAnAttributeThatDoesntExist
	{
		private ShibbolethAttributes _sut;

		private NameValueCollection CreateVariables() =>
			new NameValueCollection
			{
				{ "whatever", "whatever" },
			};

		[TestInitialize]
		public void Setup() =>
			_sut = new ShibbolethAttributes(CreateVariables());

		[TestMethod]
		public void ThenTheAttributeIsNotFound()
		{
			bool result = _sut.TryGet("iDontExist", out _);
			Assert.IsFalse(result);

			result = _sut.TryGet("SHIB_iDontExist", out _);
			Assert.IsFalse(result);

			Assert.ThrowsException<ArgumentException>(() => _sut.Get("iDontExist"));
		}
	}
}