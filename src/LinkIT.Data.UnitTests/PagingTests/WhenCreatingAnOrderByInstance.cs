using LinkIT.Data.Paging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace LinkIT.Data.UnitTests.PagingTests
{
	[TestClass]
	public class WhenCreatingAnOrderByInstance
	{
		private string[] _names = new[] { "BLABLA876", "whatever" };

		private static IEnumerable<object[]> GetInputData() =>
			new[]
			{
				// input, expectedOrderBy, expectedOrderByToString
				new object[] { "+blaBla876", new OrderBy("blaBla876", Order.ASCENDING), "+blaBla876" },
				new object[] { "BlaBla876", new OrderBy("BlaBla876", Order.ASCENDING), "+BlaBla876" },
				new object[] { "-blaBla876", new OrderBy("blaBla876", Order.DESCENDING), "-blaBla876" },
			};

		[TestMethod]
		[DynamicData(nameof(GetInputData), DynamicDataSourceType.Method)]
		public void ThenTheInstanceIsCreated(string input, OrderBy expected, string expectedToString)
		{
			bool success = OrderBy.TryParse(input, out OrderBy actual);
			Assert.IsTrue(success);
			Assert.AreEqual(expected, actual);
			Assert.IsTrue(actual.IsValidFor(_names));
			Assert.AreEqual(expectedToString, actual.ToString());
		}

		[DataTestMethod]
		[DataRow("*blaBla876;")]
		[DataRow("blaBla876;")]
		[DataRow(null)]
		[DataRow("")]
		public void ThenTheInstanceIsNotCreated(string input)
		{
			// Negative cases.
			bool success = OrderBy.TryParse(input, out _);
			Assert.IsFalse(success);
		}
	}
}