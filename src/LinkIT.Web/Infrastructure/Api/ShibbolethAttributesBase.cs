using System;
using System.Collections.Generic;

namespace LinkIT.Web.Infrastructure.Api
{
	public abstract class ShibbolethAttributesBase
	{
		protected const string SHIBBOLETH_PREFIX = "SHIB";
		protected const string IGNORE_SHIBBOLETH_KEY = "HTTP_SHIB_ATTRIBUTES";

		public const string UID_KEY = "SHIB_uid";

		protected static bool IsShibbolethKey(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return false;

			if (input.Equals(IGNORE_SHIBBOLETH_KEY, StringComparison.InvariantCultureIgnoreCase))
				return false;

			return input.IndexOf(SHIBBOLETH_PREFIX, StringComparison.InvariantCultureIgnoreCase) >= 0;
		}

		protected abstract bool InnerTryGet(string key, out string value);

		public abstract IDictionary<string, string> GetAll();

		public bool TryGet(string key, out string value)
		{
			value = null;

			if (string.IsNullOrWhiteSpace(key))
				return false;

			if (!IsShibbolethKey(key))
				return false;

			return InnerTryGet(key, out value);
		}

		public string Get(string key)
		{
			// Input validation.
			if (string.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException(nameof(key));

			if (!IsShibbolethKey(key))
				throw new ArgumentException($"{key} is not a valid Shibboleth attribute.");

			if (!TryGet(key, out var value))
				throw new ArgumentException($"Key with value '{key}' is not present as a server variable.");

			return value;
		}

		public string GetUid() =>
			Get(UID_KEY);
	}
}