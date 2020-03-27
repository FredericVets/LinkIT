using System;
using System.Web.Configuration;

namespace LinkIT.Web.Infrastructure.Api
{
	public class ConnectionString
	{
		private const string NAME = "LinkITConnectionString";

		public static string Get()
		{
			string value = WebConfigurationManager.ConnectionStrings[NAME]?.ConnectionString;
			if (string.IsNullOrWhiteSpace(value))
				throw new InvalidOperationException($"Connection string '{NAME}' was not found in config file.");

			return value;
		}
	}
}