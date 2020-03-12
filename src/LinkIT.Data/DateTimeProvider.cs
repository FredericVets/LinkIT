using System;

namespace LinkIT.Data
{
	/// <summary>
	/// Use this to remove the dependency on DateTime.Now.
	/// Makes unit testing possible.
	/// </summary>
	public static class DateTimeProvider
	{
		static DateTimeProvider()
		{
			ResetDateTime();
		}

		public static Func<DateTime> Now { get; private set; }

		public static Func<DateTime> UtcNow { get; private set; }

		public static void SetDateTime(DateTime input)
		{
			Now = () => input;
			UtcNow = () => input.ToUniversalTime();
		}

		public static void ResetDateTime()
		{
			Now = () => DateTime.Now;
			UtcNow = () => DateTime.UtcNow;
		}
	}
}