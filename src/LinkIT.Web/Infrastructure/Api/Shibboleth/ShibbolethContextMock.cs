using LinkIT.Data;
using System;
using System.Configuration;
using System.Net.Http.Headers;
using System.Web;

namespace LinkIT.Web.Infrastructure.Api.Shibboleth
{
	public class ShibbolethContextMock : IShibbolethContext
	{
		private const string APP_SETTINGS_KEY = "ShibbolethAttributesMock";

		private ShibbolethAttributesMock _mock;

		public ShibbolethContextMock(ShibbolethAttributesMock mock) =>
			_mock = mock ?? throw new ArgumentNullException(nameof(mock));

		public static bool ShouldMock
		{
			get => !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[APP_SETTINGS_KEY]);
		}

		public static ShibbolethContextMock FromConfig
		{
			get
			{
				var values = ConfigurationManager.AppSettings[APP_SETTINGS_KEY];
				var attributesMock = new ShibbolethAttributesMock(values.SplitKeyValuePairs());

				return new ShibbolethContextMock(attributesMock);
			}
		}

		public ShibbolethAttributesBase FromHeaders(HttpRequestHeaders headers) =>
			_mock;

		public ShibbolethAttributesBase FromServerVariables(HttpRequestBase request = null) =>
			_mock;
	}
}