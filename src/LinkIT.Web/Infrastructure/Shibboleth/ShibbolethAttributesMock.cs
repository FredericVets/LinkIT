using LinkIT.Data;
using System.Collections.Specialized;
using System.Configuration;

namespace LinkIT.Web.Infrastructure.Shibboleth
{
	public static class ShibbolethAttributesMock
	{
		private const string APP_SETTINGS_KEY = "ShibbolethAttributesMock.values";

		public static bool ShouldMock
		{
			get => !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[APP_SETTINGS_KEY]);
		}

		public static ShibbolethAttributes FromConfig()
		{
			var values = ConfigurationManager.AppSettings[APP_SETTINGS_KEY];
			var dict = values.SplitKeyValuePairs();

			var nameValue = new NameValueCollection();
			foreach (var kvp in dict)
				nameValue.Add(kvp.Key, kvp.Value);

			return new ShibbolethAttributes(nameValue);
		}
	}
}