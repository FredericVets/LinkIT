using LinkIT.Data;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace LinkIT.Web.Infrastructure.Api.Shibboleth
{
	public class ShibbolethAttributesMock : ShibbolethAttributes
	{
		private const string APP_SETTINGS_KEY = "ShibbolethAttributesMock";

		private readonly IDictionary<string, string> _data;

		public ShibbolethAttributesMock(IDictionary<string, string> data) =>
			_data = data ?? throw new ArgumentNullException(nameof(data));

		public static bool ShouldMock
		{
			get => !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[APP_SETTINGS_KEY]);
		}

		public static ShibbolethAttributesMock FromConfig()
		{
			var values = ConfigurationManager.AppSettings[APP_SETTINGS_KEY];

			return new ShibbolethAttributesMock(values.SplitKeyValuePairs());
		}

		protected override bool InnerTryGet(string key, out string value)
		{
			value = null;

			if (!_data.ContainsKey(key))
				return false;

			value = _data[key];

			return true;
		}

		public override IDictionary<string, string> GetAll() =>
			_data;
	}
}