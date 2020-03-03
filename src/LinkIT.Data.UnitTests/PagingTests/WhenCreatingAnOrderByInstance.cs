using LinkIT.Data.Paging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Data.UnitTests.PagingTests
{
	[TestClass]
	public class WhenCreatingAnOrderByInstance
	{
		[TestMethod]
		public void ThenTheInstanceIsCreated()
		{
			bool success = OrderBy.TryParse("+blaBla876", out OrderBy actual);
			Assert.IsTrue(success);
			Assert.AreEqual(new OrderBy("blaBla876", Order.ASCENDING), actual);

			var names = new[] { "BLABLA876", "whatever" };

			Assert.IsTrue(actual.IsValidFor(names));
			Assert.AreEqual("+blaBla876", actual.ToString());

			// When no leading '+' or '-' character is present, the default is Order.ASCENDING.
			success = OrderBy.TryParse("BlaBla876", out actual);
			Assert.IsTrue(success);
			Assert.IsTrue(actual.IsValidFor(names));
			Assert.AreEqual(new OrderBy("BlaBla876", Order.ASCENDING), actual);
			Assert.AreEqual("+BlaBla876", actual.ToString());

			success = OrderBy.TryParse("-blaBla876", out actual);
			Assert.IsTrue(success);
			Assert.IsTrue(actual.IsValidFor(names));
			Assert.AreEqual(new OrderBy("blaBla876", Order.DESCENDING), actual);
			Assert.AreEqual("-blaBla876", actual.ToString());
		}

		[TestMethod]
		public void ThenTheInstanceIsNotCreated()
		{
			// Negative cases.
			bool success = OrderBy.TryParse("*blaBla876;", out _);
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