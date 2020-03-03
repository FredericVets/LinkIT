using LinkIT.Web.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LinkIT.Web.UnitTests.ValidationTests
{
	[TestClass]
	public class WhenValidatingIfAStringIsAnElementOfAList
	{
		private StringElementOfAttribute _sut;

		[TestInitialize]
		public void Setup()
		{
			_sut = new StringElementOfAttribute(StringComparison.InvariantCultureIgnoreCase)
			{
				Elements = new[] { "azerty", "qwerty", "bla" }
			};
		}

		[TestMethod]
		public void ThenPresenceIsDetectedCorrectly()
		{
			Assert.IsTrue(_sut.IsValid("bla"));
			Assert.IsTrue(_sut.IsValid("blA"));
			Assert.IsTrue(_sut.IsValid("BLA"));
		}

		[TestMethod]
		public void ThenAbsenceIsDetectedCorrectly()
		{
			Assert.IsFalse(_sut.IsValid("doesntExist"));
		}
	}
}
