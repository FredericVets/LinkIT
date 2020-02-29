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
			OrderBy actual;

			success = OrderBy.TryParse("+blaBla876", out actual);
			Assert.IsTrue(success);
			Assert.AreEqual(new OrderBy("blaBla876", Order.ASCENDING), actual);
			Assert.AreEqual("+blaBla876", actual.ToString());

			// When no leading '+' or '-' character is present, the default is Order.ASCENDING.
			success = OrderBy.TryParse("blaBla876", out actual);
			Assert.IsTrue(success);
			Assert.AreEqual(new OrderBy("blaBla876", Order.ASCENDING), actual);
			Assert.AreEqual("+blaBla876", actual.ToString());

			success = OrderBy.TryParse("-blaBla876", out actual);
			Assert.IsTrue(success);
			Assert.AreEqual(new OrderBy("blaBla876", Order.DESCENDING), actual);
			Assert.AreEqual("-blaBla876", actual.ToString());

			// Negative cases.
			success = OrderBy.TryParse("*blaBla876;", out actual);
			Assert.IsFalse(success);

			success = OrderBy.TryParse("blaBla876;", out actual);
			Assert.IsFalse(success);

			success = OrderBy.TryParse(null, out actual);
			Assert.IsFalse(success);

			success = OrderBy.TryParse("", out actual);
			Assert.IsFalse(success);
		}
	}
}