using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace LinkIT.Web.Infrastructure.Auth
{
	public class JsonWebTokenWrapper : IJsonWebTokenWrapper
	{
		private readonly HttpHeadersWrapper _httpHeaders;
		private readonly JsonWebKeySetWrapper _keySet;
		private readonly bool _validateLifetime;

		private JwtSecurityToken _validatedToken;

		public JsonWebTokenWrapper(
			HttpHeadersWrapper httpHeaders, JsonWebKeySetWrapper keySet, bool validateLifeTime = true)
		{
			_httpHeaders = httpHeaders ?? throw new ArgumentNullException(nameof(httpHeaders));
			_keySet = keySet ?? throw new ArgumentNullException(nameof(keySet));
			_validateLifetime = validateLifeTime;
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
			if (string.IsNullOrWhiteSpace(value))
				return false;

			return true;
		}

		/// <summary>
		/// The user agent should send the JWT, typically in the Authorization header using the Bearer schema. 
		/// The content of the header should look like the following:
		/// Authorization: Bearer <token>
		/// </summary>
		/// <returns></returns>

		public string Scope => GetPayloadValue("scope");

		public string UserId => GetPayloadValue("preferred_username");

		public string Name => GetPayloadValue("name");

		public string GivenName => GetPayloadValue("given_name");

		public string FamilyName => GetPayloadValue("family_name");

		public string Email => GetPayloadValue("email");

		// Throws when not valid.
		public void Validate()
		{
			string rawJwt = _httpHeaders.GetRawJwt();

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