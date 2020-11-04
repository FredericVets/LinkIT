using Microsoft.IdentityModel.Tokens;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LinkIT.Web.Infrastructure.Auth
{
	public class JsonWebKeySetWrapper
	{
		public const string APP_SETTINGS_KEY = "jwks.url";

		public JsonWebKeySetWrapper(string jwksJson)
		{
			if (string.IsNullOrWhiteSpace(jwksJson))
				throw new ArgumentNullException(nameof(jwksJson));

			JsonWebKeySet = new JsonWebKeySet(jwksJson);
		}

		public static Task<JsonWebKeySetWrapper> FromUrl() =>
			FromUrl(ConfigurationManager.AppSettings[APP_SETTINGS_KEY]);

		public static async Task<JsonWebKeySetWrapper> FromUrl(string jwksUrl)
		{
			if (string.IsNullOrWhiteSpace(jwksUrl))
				throw new ArgumentNullException(nameof(jwksUrl));

			var handler = new HttpClientHandler { UseProxy = false };
			using (var client = new HttpClient(handler))
			{
				var response = await client.GetAsync(jwksUrl);
				if (response.StatusCode != HttpStatusCode.OK)
					throw new InvalidOperationException(
						$"Failed to fetch the jwks from {jwksUrl} with status code : {response.StatusCode}.");

				return new JsonWebKeySetWrapper(await response.Content.ReadAsStringAsync());
			}
		}

		public JsonWebKeySet JsonWebKeySet { get; private set; }
	}
}