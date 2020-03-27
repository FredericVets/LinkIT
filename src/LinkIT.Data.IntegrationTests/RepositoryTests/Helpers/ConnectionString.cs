using System;
using System.Configuration;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.Helpers
{
	public class ConnectionString
	{
		private const string NAME = "LinkITConnectionString";

		public static string Get()
		{
			string value = ConfigurationManager.ConnectionStrings[NAME]?.ConnectionString;
			if (string.IsNullOrWhiteSpace(value))
				throw new InvalidOperationException($"Connection string '{NAME}' was not found in config file.");

			return value;
		}
	}
}