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
		public void Setup() =>
			_sut = new StringElementOfAttribute(StringComparison.InvariantCultureIgnoreCase)
			{
				Elements = new[] { "azerty", "qwerty", "bla" }
			};

		[DataTestMethod]
		[DataRow("bla")]
		[DataRow("blA")]
		[DataRow("BLA")]
		public void ThenPresenceIsDetectedCorrectly(string input) =>
			Assert.IsTrue(_sut.IsValid(input));

		[TestMethod]
		public void ThenAbsenceIsDetectedCorrectly() =>
			Assert.IsFalse(_sut.IsValid("doesntExist"));
	}
}