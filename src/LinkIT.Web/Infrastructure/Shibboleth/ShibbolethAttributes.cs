using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace LinkIT.Web.Infrastructure.Shibboleth
{
	/// <summary>
	/// These attributes are available in datastructures of the current Request object.
	/// They get injected into the IIS pipeline by the Shibboleth middleware.
	/// 
	/// A mock implementation is available : ShibbolethAttributesMock
	/// This can be used in test environment where the Shibboleth middleware isn't available.
	/// </summary>
	public class ShibbolethAttributes
	{
		private const string SHIBBOLETH_PREFIX = "SHIB";
		private const string IGNORE_SHIBBOLETH_KEY = "HTTP_SHIB_ATTRIBUTES";

		public const string UID_KEY = "SHIB_uid";

		private readonly NameValueCollection _data;

		public ShibbolethAttributes(NameValueCollection data) =>
			_data = data ?? throw new ArgumentNullException(nameof(data));

		/// <summary>
		/// Shibboleth documentation at https://wiki.shibboleth.net/confluence/display/SP3/AttributeAccess
		/// recommends to use the headers to retrieve the Shibboleth attributes instead of the server variables collection.
		/// </summary>
		/// <returns>The ShibbolethAttributes</returns>
		public static ShibbolethAttributes FromHeader() =>
			new ShibbolethAttributes(HttpContext.Current.Request.Headers);

		public static ShibbolethAttributes FromServerVariables() =>
			new ShibbolethAttributes(HttpContext.Current.Request.ServerVariables);

		private static bool IsShibbolethKey(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return false;

			if (input.Equals(IGNORE_SHIBBOLETH_KEY, StringComparison.InvariantCultureIgnoreCase))
				return false;

			return input.StartsWith(SHIBBOLETH_PREFIX, StringComparison.InvariantCultureIgnoreCase);
		}

		private bool InnerTryGet(string key, out string value)
		{
			value = null;

			string temp = _data[key];
			if (string.IsNullOrWhiteSpace(temp))
				return false;

			value = temp;

			return true;
		}

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

			if (!InnerTryGet(key, out var value))
				throw new ArgumentException($"Key with value '{key}' is not present as a server variable.");

			return value;
		}

		public bool TryGetUid(out string uid) =>
			TryGet(UID_KEY, out uid);

		public string GetUid() =>
			Get(UID_KEY);

		public IDictionary<string, string> GetAll()
		{
			var result = new Dictionary<string, string>();
			if (!_data.HasKeys())
				return result;

			foreach (string key in _data.AllKeys)
			{
				if (!IsShibbolethKey(key))
					continue;

				result.Add(key, _data[key]);
			}

			return result;
		}
	}
}