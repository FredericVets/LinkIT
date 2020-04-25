using LinkIT.Data.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Data.UnitTests.StringExtensionTests
{
	[TestClass]
	public class WhenSplittingCommaSeparated
	{
		[DataTestMethod]
		[DataRow("first,second,third,fourth", "first,second,third,fourth")]
		[DataRow("first,second   ,     third,fourth,,", "first,second,third,fourth")]
		[DataRow(",first,second,third,fourth", "first,second,third,fourth")]
		[DataRow(",,,    ,  ,first, , second,third,fourth", "first,second,third,fourth")]
		public void ThenTheSplittingBehavesAsExpected(string input, string expected)
		{
			var actual = input.SplitCommaSeparated();
			string rejoined = string.Join(",", actual);

			Assert.AreEqual(expected, rejoined);
		}
	}
}