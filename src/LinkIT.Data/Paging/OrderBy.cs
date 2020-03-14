using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace LinkIT.Data.Paging
{
	/// <summary>
	/// Suppose you want to order on a field called 'name'.
	/// This class supports the syntax +name for ascending sort, -name for descending sort.
	/// When no ordering is provided, it defaults to ascending.
	/// So +name = name.
	/// Handles the parsing of this format.
	/// </summary>
	[TypeConverter(typeof(OrderByConverter))]
	public class OrderBy : IEquatable<OrderBy>
	{
		private const char ASCENDING_CHARACTER = '+';
		private const char DESCENDING_CHARACTER = '-';
		private const string PLAIN_STRING_PATTERN = @"^\w+$";

		public const string REGEX_PATTERN = @"^(\+|\-)?\w+$";

		public OrderBy(string name, Order order)
		{
			Name = name ?? throw new ArgumentNullException("name");

			if (!Regex.IsMatch(name, PLAIN_STRING_PATTERN))
				throw new ArgumentException("name");

			Order = order;
		}

		public string Name { get; }

		public Order Order { get; }

		public static bool operator ==(OrderBy left, OrderBy right)
		{
			return EqualityComparer<OrderBy>.Default.Equals(left, right);
		}

		public static bool operator !=(OrderBy left, OrderBy right) => !(left == right);

		public static OrderBy Parse(string input)
		{
			if (!TryParse(input, out OrderBy result))
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
			if (IsSortingChar(first))
			{
				result = new OrderBy(input.Substring(1), GetOrderFor(first));

				return true;
			}

			result = new OrderBy(input, GetOrderFor(first));

			return true;

			// Local functions.
			bool IsSortingChar(char value)
			{
				return new[] { ASCENDING_CHARACTER, DESCENDING_CHARACTER }.Contains(value);
			}

			Order GetOrderFor(char value)
			{
				if (value == ASCENDING_CHARACTER)
					return Order.ASCENDING;

				if (value == DESCENDING_CHARACTER)
					return Order.DESCENDING;

				// The default.
				return Order.ASCENDING;
			}
		}

		public override bool Equals(object obj) => Equals(obj as OrderBy);

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

			// Local function.
			char GetCharFor(Order input)
			{
				if (input == Order.ASCENDING)
					return ASCENDING_CHARACTER;

				if (input == Order.DESCENDING)
					return DESCENDING_CHARACTER;

				throw new InvalidOperationException($"Unknown Order enum value : {input}.");
			}
		}

		public bool Equals(OrderBy other)
		{
			return other != null &&
				Name == other.Name &&
				Order == other.Order;
		}

		public bool IsValidFor(IEnumerable<string> names)
		{
			return names.Contains(Name, StringComparer.InvariantCultureIgnoreCase);
		}
	}
}