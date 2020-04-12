using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Data.UnitTests.StringExtensionTests
{
	[TestClass]
	public class WhenSplittingKeyValuePairs
	{
		[TestMethod]
		public void ThenTheSplittingBehavesAsExpected()
		{
			var actual = "a: b, c: d,   e  :  f".SplitKeyValuePairs();

			Assert.AreEqual(3, actual.Count);
			
			Assert.AreEqual("a", actual.ElementAt(0).Key);
			Assert.AreEqual("b", actual.ElementAt(0).Value);
			Assert.AreEqual("c", actual.ElementAt(1).Key);
			Assert.AreEqual("d", actual.ElementAt(1).Value);
			Assert.AreEqual("e", actual.ElementAt(2).Key);
			Assert.AreEqual("f", actual.ElementAt(2).Value);
		}
	}
}