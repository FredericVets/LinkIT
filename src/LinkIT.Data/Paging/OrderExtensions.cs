using System;

namespace LinkIT.Data.Paging
{
	public static class OrderExtensions
	{
		public static string ForSql(this Order input)
		{
			if (input == Order.ASCENDING)
				return "ASC";

			if (input == Order.DESCENDING)
				return "DESC";

			throw new InvalidOperationException($"Unknown Order enum value : {input}.");
		}
	}
}