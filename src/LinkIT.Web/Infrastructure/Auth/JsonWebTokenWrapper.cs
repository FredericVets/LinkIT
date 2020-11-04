using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Web;

namespace LinkIT.Web.Infrastructure.Auth
{
	public class JsonWebTokenWrapper : IJsonWebTokenWrapper
	{
		public const string AUTHORIZATION_HEADER = "Authorization";
		public const string BEARER = "Bearer";

		private readonly NameValueCollection _httpHeaders;
		private readonly JsonWebKeySetWrapper _keySet;
		private readonly bool _validateLifetime;

		private JwtSecurityToken _validatedToken;

		public JsonWebTokenWrapper(
			NameValueCollection httpHeaders, JsonWebKeySetWrapper keySet, bool validateLifeTime = true)
		{
			_httpHeaders = httpHeaders ?? throw new ArgumentNullException(nameof(httpHeaders));
			_keySet = keySet ?? throw new ArgumentNullException(nameof(keySet));
			_validateLifetime = validateLifeTime;
		}

		private static string ExtractJwtFrom(string authorizationHeader)
		{
			if (!authorizationHeader.StartsWith(BEARER, StringComparison.InvariantCultureIgnoreCase))
				throw new InvalidOperationException("Authorization header doesn't follow the Bearer schema.");

			return authorizationHeader.Substring(BEARER.Length + 1);
		}

		private string GetAuthorizationHeader()
		{
			string auth = _httpHeaders[AUTHORIZATION_HEADER];
			if (string.IsNullOrWhiteSpace(auth))
				throw new InvalidOperationException("No Authorization header present.");

			return auth;
		}

		private string GetPayloadValue(string key)
		{
			if (!TryGetPayloadValue(key, out var value))
				throw new KeyNotFoundException($"key: {key}");

			return value;
		}

		private bool TryGetPayloadValue(string key, out string value)
		{
			if (_validatedToken == null)
				Validate();

			value = null;
			if (!_validatedToken.Payload.ContainsKey(key))
				return false;

			value = (string)_validatedToken.Payload[key];

			return true;
		}

		public static JsonWebTokenWrapper FromHeaders() =>
			new JsonWebTokenWrapper(
				HttpContext.Current.Request.Headers,
				JsonWebKeySetWrapper.FromUrl().Result);

		/// <summary>
		/// The user agent should send the JWT, typically in the Authorization header using the Bearer schema. 
		/// The content of the header should look like the following:
		/// Authorization: Bearer <token>
		/// </summary>
		/// <returns></returns>

		public string Scope { get => GetPayloadValue("scope"); }

		public string Name { get => GetPayloadValue("name"); }

		public string UserId { get => GetPayloadValue("preferred_username"); }

		public string GivenName { get => GetPayloadValue("given_name"); }

		public string FamilyName { get => GetPayloadValue("family_name"); }

		public string Email { get => GetPayloadValue("email"); }

		// Throws when not valid.
		public void Validate()
		{
			string authHeader = GetAuthorizationHeader();
			string rawJwt = ExtractJwtFrom(authHeader);

			var validationParams = new TokenValidationParameters
			{
				IssuerSigningKey = _keySet.JsonWebKeySet.Keys.First(),
				ValidateLifetime = _validateLifetime,
				ValidateAudience = false,
				ValidateIssuer = false
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			tokenHandler.ValidateToken(rawJwt, validationParams, out var validatedToken);

			_validatedToken = (JwtSecurityToken)validatedToken;
		}

		public bool TryGetUserId(out string userId) => TryGetPayloadValue("preferred_username", out userId);
	}
}