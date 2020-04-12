using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace LinkIT.Web.Infrastructure.Api.Shibboleth
{
	/// <summary>
	/// Shibboleth documentation at https://wiki.shibboleth.net/confluence/display/SP3/AttributeAccess
	/// recommends to use the headers to retrieve the Shibbolet attributes instead of the server variables collection.
	/// </summary>
	public class ShibbolethHeaders : ShibbolethAttributesBase
	{
		private readonly HttpRequestHeaders _headers;

		public ShibbolethHeaders(HttpRequestHeaders headers) =>
			_headers = headers ?? throw new ArgumentNullException(nameof(headers));

		private bool HasHeaders() =>
			_headers.Any();

		protected override bool InnerTryGet(string key, out string value)
		{
			value = null;

			if (!HasHeaders())
				return false;

			if (!_headers.TryGetValues(key, out var values))
				return false;

			value = values.First();

			return true;
		}

		public override IDictionary<string, string> GetAll()
		{
			var result = new Dictionary<string, string>();
			if (!HasHeaders())
				return result;

			foreach (var kvp in _headers)
			{
				if (!IsShibbolethKey(kvp.Key))
					continue;

				result.Add(kvp.Key, kvp.Value.First());
			}

			return result;
		}
	}
}