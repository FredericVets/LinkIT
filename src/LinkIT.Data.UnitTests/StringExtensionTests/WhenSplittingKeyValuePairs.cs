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
			var actual = "a: b, c: d,   e  :  f".SplitKeyValuePairs().ToArray();

			Assert.AreEqual(3, actual.Length);
			
			Assert.AreEqual("a", actual[0].Key);
			Assert.AreEqual("b", actual[0].Value);
			Assert.AreEqual("c", actual[1].Key);
			Assert.AreEqual("d", actual[1].Value);
			Assert.AreEqual("e", actual[2].Key);
			Assert.AreEqual("f", actual[2].Value);
		}
	}
}