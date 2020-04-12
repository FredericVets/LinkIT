using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Data.UnitTests.StringExtensionTests
{
	[TestClass]
	public class WhenSplittingForSeparator
	{
		[DataTestMethod]
		[DataRow("whatever: whatever1", ':',  "whatever,whatever1")]
		[DataRow("x: y", ':', "x,y")]
		public void ThenTheSplittingBehavesAsExpected(string input, char splitChar, string expected)
		{
			var actual = input.SplitForSeparator(':');
			string rejoined = string.Join(",", actual);

			Assert.AreEqual(expected, rejoined);
		}
	}
}