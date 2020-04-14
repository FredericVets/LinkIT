using LinkIT.Data;
using System;
using System.Configuration;
using System.Linq;

namespace LinkIT.Web.Infrastructure.Api.Shibboleth.Auth
{
	public class ShibbolethAuthorizerMock : IShibbolethAuthorizer
	{
		private const string APP_SETTINGS_KEY = "ShibbolethAuthorizerMock.roles";
		
		private readonly string[] _mockRoles;

		public ShibbolethAuthorizerMock(string[] mockRoles) =>
			_mockRoles = mockRoles ?? throw new ArgumentNullException(nameof(mockRoles));

		public static bool ShouldMock
		{
			get => !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[APP_SETTINGS_KEY]);
		}

		public static ShibbolethAuthorizerMock FromConfig()
		{
			var values = ConfigurationManager.AppSettings[APP_SETTINGS_KEY];
			var roles = values.SplitCommaSeparated();

			return new ShibbolethAuthorizerMock(roles);
		}

		public bool IsAuthorized(string user, params string[] requiredRoles)
		{
			if (requiredRoles == null || !requiredRoles.Any())
				throw new ArgumentNullException(nameof(requiredRoles));

			return requiredRoles.All(r => _mockRoles.Contains(r, StringComparer.InvariantCultureIgnoreCase));
		}
	}
}