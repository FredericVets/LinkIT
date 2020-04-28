using System;

namespace LinkIT.Data.Queries
{
	public class DateRange
	{
		public DateRange(DateTime? startDate, DateTime? endDate)
		{
			Validate(startDate, endDate);

			StartDate = startDate;
			EndDate = endDate;
		}

		private static void Validate(DateTime? startDate, DateTime? endDate)
		{
			if (!startDate.HasValue && !endDate.HasValue)
				throw new ArgumentNullException($"Both {nameof(startDate)} and {nameof(endDate)} are not specified.");

			if (startDate.HasValue && endDate.HasValue && startDate > endDate)
				throw new ArgumentException($"{nameof(startDate)} is greater than {nameof(endDate)}.");
		}

		public DateTime? StartDate { get; }

		public DateTime? EndDate { get; }
	}
}