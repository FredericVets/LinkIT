using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace LinkIT.Web.Infrastructure.Auth
{
	public class JsonWebTokenWrapper
	{
		private string _rawToken;
		private JsonWebKeySetWrapper _keySet;
		private bool _validateLifetime;

		private JwtSecurityToken _validatedToken;

		public JsonWebTokenWrapper(string rawToken, JsonWebKeySetWrapper keySet, bool validateLifeTime = true)
		{
			if (string.IsNullOrWhiteSpace(rawToken))
				throw new ArgumentNullException(nameof(rawToken));

			_rawToken = rawToken;
			_keySet = keySet ?? throw new ArgumentNullException(nameof(keySet));
			_validateLifetime = validateLifeTime;
		}

		private string GetPayloadValue(string key)
		{
			if (_validatedToken == null)
				Validate();

			return (string)_validatedToken.Payload[key];
		}

		private string GetAuthorizationHeader()
		{
			// And correctly extract the bearer token.

			throw new NotImplementedException();
		}

		public string Scope { get => GetPayloadValue("scope"); }

		public string Name { get => GetPayloadValue("name"); }

		public string userId { get => GetPayloadValue("preferred_username"); }

		public string GivenName { get => GetPayloadValue("given_name"); }

		public string FamilyName { get => GetPayloadValue("family_name"); }

		public string Email { get => GetPayloadValue("email"); }

		// Throws when not valid.
		public void Validate()
		{
			// TODO : get jwt from http header.

			var validationParams = new TokenValidationParameters
			{
				IssuerSigningKey = _keySet.JsonWebKeySet.Keys.First(),
				ValidateLifetime = _validateLifetime,
				ValidateAudience = false,
				ValidateIssuer = false
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			tokenHandler.ValidateToken(_rawToken, validationParams, out var validatedToken);

			_validatedToken = (JwtSecurityToken)validatedToken;
		}
	}
}