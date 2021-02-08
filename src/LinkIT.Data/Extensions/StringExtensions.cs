using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIT.Data.Extensions
{
	public static class StringExtensions
	{
		public static string[] SplitForSeparator(this string input, char separator)
		{
			if (string.IsNullOrWhiteSpace(input))
				throw new ArgumentNullException(nameof(input));

			return input.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
				.Select(x => x.Trim())
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.ToArray();
		}

		public static string[] SplitCommaSeparated(this string input) =>
			input.SplitForSeparator(',');

		/// <summary>
		/// Splits an input string formatted like "key1: value1, key2: value2" into a dictionary.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static IDictionary<string, string> SplitKeyValuePairs(this string input) =>
			input.SplitCommaSeparated()
				.ToDictionary(
					x => x.SplitForSeparator(':')[0],
					x => x.SplitForSeparator(':')[1]);
	}
}