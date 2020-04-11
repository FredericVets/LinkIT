using System;
using System.Configuration;

namespace LinkIT.Data
{
	public class ConnectionString
	{
		private const string NAME = "LinkITConnectionString";

		public ConnectionString()
		{
			string value = ConfigurationManager.ConnectionStrings[NAME]?.ConnectionString;
			if (string.IsNullOrWhiteSpace(value))
				throw new InvalidOperationException($"Connection string '{NAME}' was not found in config file.");

			Value = value;
		}

		public string Value { get; }
	}
}