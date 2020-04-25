using LinkIT.Data.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Data.UnitTests.StringExtensionTests
{
	[TestClass]
	public class WhenSplittingForSeparator
	{
		[DataTestMethod]
		[DataRow("whatever: whatever1", ':',  "whatever,whatever1")]
		[DataRow("x    ; y   ;   ;z  ;", ';', "x,y,z")]
		[DataRow("x: x", ':', "x,x")]
		public void ThenTheSplittingBehavesAsExpected(string input, char splitChar, string expected)
		{
			var actual = input.SplitForSeparator(splitChar);
			string rejoined = string.Join(",", actual);

			Assert.AreEqual(expected, rejoined);
		}
	}
}