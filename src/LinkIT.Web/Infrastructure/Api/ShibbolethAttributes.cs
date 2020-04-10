using System;
using System.Collections.Generic;
using System.Web;

namespace LinkIT.Web.Infrastructure.Api
{
	public class ShibbolethAttributes
	{
		private const string SHIBBOLETH_PREFIX = "SHIB";
		private const string IGNORE_SHIBBOLETH_KEY = "HTTP_SHIB_ATTRIBUTES";
		
		public const string UID_KEY = "SHIB_uid";

		private readonly HttpContextBase _context;

		public ShibbolethAttributes() : this(new HttpContextWrapper(HttpContext.Current)) { }

		public ShibbolethAttributes(HttpContextBase context) =>
			_context = context ?? throw new ArgumentNullException(nameof(context));

		private static bool IsShibbolethKey(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return false;

			if (input.Equals(IGNORE_SHIBBOLETH_KEY, StringComparison.InvariantCultureIgnoreCase))
				return false;

			return input.IndexOf(SHIBBOLETH_PREFIX, StringComparison.InvariantCultureIgnoreCase) >= 0;
		}

		private bool HasServerVariables()
		{
			var serverVariables = _context?.Request?.ServerVariables;
			if (serverVariables == null)
				return false;

			if (!serverVariables.HasKeys())
				return false;

			return true;
		}

		public bool TryGet(string key, out string value)
		{
			value = null;

			if (string.IsNullOrWhiteSpace(key))
				return false;

			if (!IsShibbolethKey(key))
				return false;

			if (!HasServerVariables())
				return false;

			var serverVariables = _context.Request.ServerVariables;
			string temp = serverVariables[key];
			if (string.IsNullOrWhiteSpace(temp))
				return false;

			value = temp;

			return true;
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

		public IDictionary<string, string> GetAll()
		{
			var result = new Dictionary<string, string>();
			if (!HasServerVariables())
				return result;

			var serverVariables = _context.Request.ServerVariables;
			foreach (string key in serverVariables.AllKeys)
			{
				if (!IsShibbolethKey(key))
					continue;

				result.Add(key, serverVariables[key]);
			}

			return result;
		}
	}
}