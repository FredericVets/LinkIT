using LinkIT.Web.Infrastructure.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Specialized;

namespace LinkIT.Web.UnitTests.Infrastructure.Api.ShibbolethServerVariablesTests
{
	[TestClass]
	public class WhenGettingAVariableThatDoesntExist
	{
		private ShibbolethServerVariables _sut;

		private NameValueCollection CreateVariables() =>
			new NameValueCollection()
			{
				{ "whatever", "whatever" },
			};

		[TestInitialize]
		public void Setup()
		{
			var mock = MockFactory.Create(CreateVariables);

			_sut = new ShibbolethServerVariables(mock.Object);
		}

		[TestMethod]
		public void ThenTheVariableIsNotFound()
		{
			bool result = _sut.TryGet("iDontExist", out _);
			Assert.IsFalse(result);

			result = _sut.TryGet("SHIB_iDontExist", out _);
			Assert.IsFalse(result);

			Assert.ThrowsException<ArgumentException>(() => _sut.Get("iDontExist"));
		}
	}
}