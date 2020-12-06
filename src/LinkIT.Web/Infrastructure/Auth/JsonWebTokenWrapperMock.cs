using System;
using System.Configuration;

namespace LinkIT.Web.Infrastructure.Auth
{
	public class JsonWebTokenWrapperMock : IJsonWebTokenWrapper
	{
		public static bool ShouldMock
		{
			get
			{
				string value = ConfigurationManager.AppSettings["jwt.should_mock"];

				return string.Equals(value, "true", StringComparison.InvariantCultureIgnoreCase);
			}
		}

		public string Scope => ConfigurationManager.AppSettings["jwt.mock.scope"];

		public string UserId => ConfigurationManager.AppSettings["jwt.mock.preferred_username"];

		public string Name => ConfigurationManager.AppSettings["jwt.mock.name"];

		public string GivenName => ConfigurationManager.AppSettings["jwt.mock.given_name"];

		public string FamilyName => ConfigurationManager.AppSettings["jwt.mock.family_name"];

		public string Email => ConfigurationManager.AppSettings["jwt.mock.email"];

		public void Validate() { }

		public bool TryGetUserId(out string userId)
		{
			userId = UserId;

			return true;
		}

		public bool TryGetScope(out string scope)
		{
			scope = Scope;

			return true;
		}
	}
}