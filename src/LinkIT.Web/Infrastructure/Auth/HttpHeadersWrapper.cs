using System;
using System.Collections.Specialized;
using System.Web;

namespace LinkIT.Web.Infrastructure.Auth
{
	public class HttpHeadersWrapper
	{
		public const string AUTHORIZATION_HEADER = "Authorization";
		public const string BEARER = "Bearer";

		private readonly NameValueCollection _headers;

		public HttpHeadersWrapper(NameValueCollection headers) =>
			_headers = headers ?? throw new ArgumentNullException(nameof(headers));

		private static string ExtractJwtFrom(string authorizationHeader)
		{
			if (!authorizationHeader.StartsWith($"{BEARER} ", StringComparison.InvariantCultureIgnoreCase))
				throw new InvalidOperationException("Authorization header doesn't follow the Bearer schema.");

			return authorizationHeader.Substring(BEARER.Length + 1);
		}

		/// <summary>
		/// Header should be of form 'Authorization: <type> <credentials>'.
		/// So for us it must be : 'Authorization: Bearer <jwt>'.
		/// </summary>
		/// <returns></returns>
		private string GetAuthorizationHeader()
		{
			string authHeader = _headers[AUTHORIZATION_HEADER];
			if (string.IsNullOrWhiteSpace(authHeader))
				throw new InvalidOperationException("No Authorization header present.");

			return authHeader;
		}

		public static HttpHeadersWrapper FromCurrentContext() =>
			new HttpHeadersWrapper(HttpContext.Current.Request.Headers);

		public string GetRawJwt()
		{
			string authHeader = GetAuthorizationHeader();

			return ExtractJwtFrom(authHeader);
		}
	}
}