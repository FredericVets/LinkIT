using LinkIT.Data.Paging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Data.UnitTests.PagingTests
{
	[TestClass]
	public class WhenCreatingAnOrderByInstance
	{
		[TestMethod]
		public void ThenTheResultIsAsExpected()
		{
			bool success;

			success = OrderBy.TryParse("+blaBla876", out OrderBy actual);
			Assert.IsTrue(success);
			Assert.AreEqual(new OrderBy("blaBla876", Order.ASCENDING), actual);
			Assert.AreEqual("+blaBla876", actual.ToString());

			// When no leading '+' or '-' character is present, the default is Order.ASCENDING.
			success = OrderBy.TryParse("BlaBla876", out actual);
			Assert.IsTrue(success);
			Assert.AreEqual(new OrderBy("BlaBla876", Order.ASCENDING), actual);
			Assert.AreEqual("+BlaBla876", actual.ToString());

			success = OrderBy.TryParse("-blaBla876", out actual);
			Assert.IsTrue(success);
			Assert.AreEqual(new OrderBy("blaBla876", Order.DESCENDING), actual);
			Assert.AreEqual("-blaBla876", actual.ToString());

			// Negative cases.
			success = OrderBy.TryParse("*blaBla876;", out _);
			Assert.IsFalse(success);

			success = OrderBy.TryParse("blaBla876;", out _);
			Assert.IsFalse(success);

			success = OrderBy.TryParse(null, out _);
			Assert.IsFalse(success);

			success = OrderBy.TryParse("", out _);
			Assert.IsFalse(success);
		}
	}
}