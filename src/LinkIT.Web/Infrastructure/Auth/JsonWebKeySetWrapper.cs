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
		private const string APP_SETTINGS_KEY = "jwksJson";

		public JsonWebKeySetWrapper() : this(ConfigurationManager.AppSettings[APP_SETTINGS_KEY]) { }

		public JsonWebKeySetWrapper(string jwksJson)
		{
			if (string.IsNullOrWhiteSpace(jwksJson))
				throw new ArgumentNullException(nameof(jwksJson));

			JsonWebKeySet = new JsonWebKeySet(jwksJson);
		}


		public static async Task<JsonWebKeySetWrapper> Fetch(string jwksUrl)
		{
			using (var client = new HttpClient())
			{
				var response = await client.GetAsync(jwksUrl);
				if (response.StatusCode != HttpStatusCode.OK)
					throw new InvalidOperationException($"Failed to fetch the jwks from {jwksUrl}.");

				return new JsonWebKeySetWrapper(await response.Content.ReadAsStringAsync());
			}
		}

		public JsonWebKeySet JsonWebKeySet { get; private set; }
	}
}