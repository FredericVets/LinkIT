﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace LinkIT.Web.Infrastructure.Auth
{
	public class JsonWebKeySetWrapper
	{
		public const string JWKS_URL_KEY = "jwks.url";
		public const string JWKS_JSON_KEY = "jwks.json";

		public JsonWebKeySetWrapper(string jwksJson)
		{
			if (string.IsNullOrWhiteSpace(jwksJson))
				throw new ArgumentNullException(nameof(jwksJson));

			JsonWebKeySet = new JsonWebKeySet(jwksJson);
		}

		public static JsonWebKeySetWrapper FromConfig() =>
			new JsonWebKeySetWrapper(ConfigurationManager.AppSettings[JWKS_JSON_KEY]);

		public static Task<JsonWebKeySetWrapper> FromUrl() =>
			FromUrl(ConfigurationManager.AppSettings[JWKS_URL_KEY]);

		public static async Task<JsonWebKeySetWrapper> FromUrl(string jwksUrl)
		{
			if (string.IsNullOrWhiteSpace(jwksUrl))
				throw new ArgumentNullException(nameof(jwksUrl));

			using (var handler = new HttpClientHandler { UseProxy = false, UseCookies = false })
			using (var client = new HttpClient(handler))
			using (var response = await client.GetAsync(jwksUrl))
			{
				if (!response.IsSuccessStatusCode)
					throw new InvalidOperationException(
						$"Failed to fetch the jwks from {jwksUrl} with status code : {response.StatusCode}.");

				var content = await response.Content.ReadAsStringAsync();

				return new JsonWebKeySetWrapper(content);
			}
		}

		public JsonWebKeySet JsonWebKeySet { get; private set; }
	}
}