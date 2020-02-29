using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LinkIT.Data.Paging
{
	public class OrderBy : IEquatable<OrderBy>
	{
		private const char ASCENDING_CHARACTER = '+';
		private const char DESCENDING_CHARACTER = '-';
		private const string PLAIN_STRING_PATTERN = @"^\w+$";

		public const string REGEX_PATTERN = @"^(\+|\-)?\w+$";

		public OrderBy(string name, Order order)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException("name");

			if (!Regex.IsMatch(name, PLAIN_STRING_PATTERN))
				throw new ArgumentException("name");

			Name = name;
			Order = order;
		}

		private static Order GetOrderFor(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				throw new ArgumentNullException("input");

			char first = input.First();

			if (first == ASCENDING_CHARACTER)
				return Order.ASCENDING;

			if (first == DESCENDING_CHARACTER)
				return Order.DESCENDING;

			// The default.
			return Order.ASCENDING;
		}

		private static char GetCharFor(Order input)
		{
			if (input == Order.ASCENDING)
				return ASCENDING_CHARACTER;

			if (input == Order.DESCENDING)
				return DESCENDING_CHARACTER;

			throw new InvalidOperationException($"Unknown Order enum value : {input}.");
		}

		public static bool operator ==(OrderBy left, OrderBy right)
		{
			return EqualityComparer<OrderBy>.Default.Equals(left, right);
		}

		public static bool operator !=(OrderBy left, OrderBy right)
		{
			return !(left == right);
		}

		public string Name { get; }

		public Order Order { get; }

		public static OrderBy Parse(string input)
		{
			OrderBy result;
			if (!TryParse(input, out result))
				throw new ArgumentException($"'{input}' is an invalid format.");

			return result;
		}

		public static bool TryParse(string input, out OrderBy result)
		{
			result = null;

			if (string.IsNullOrWhiteSpace(input))
				return false;

			if (!Regex.IsMatch(input, REGEX_PATTERN))
				return false;

			char first = input.First();
			if (new[] { ASCENDING_CHARACTER, DESCENDING_CHARACTER }.Contains(first))
			{
				result = new OrderBy(input.Substring(1), GetOrderFor(input));
			}
			else
			{
				result = new OrderBy(input, GetOrderFor(input));
			}

			return true;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as OrderBy);
		}

		public override int GetHashCode()
		{
			var hashCode = 956886061;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
			hashCode = hashCode * -1521134295 + Order.GetHashCode();

			return hashCode;
		}

		public override string ToString()
		{
			char prefix = GetCharFor(Order);

			return $"{prefix}{Name}";
		}

		public bool Equals(OrderBy other)
		{
			return other != null &&
				   Name == other.Name &&
				   Order == other.Order;
		}
	}
}