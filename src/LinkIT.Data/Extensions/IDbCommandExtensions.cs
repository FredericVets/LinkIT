using System;
using System.Data;
using System.Data.SqlClient;

namespace LinkIT.Data.Extensions
{
	/// <summary>
	/// This is a convenience method for adding a SqlParameter to an IDbCommand instance.
	/// This IDbCommand instance is used in client code to facilitate unit testing.
	/// This approach is used because SqlCommand is a sealed class and can not be mocked.
	/// <see href="https://stackoverflow.com/questions/6376715/how-to-mock-sqlparametercollection-using-moq"/>
	/// </summary>
	public static class IDbCommandExtensions
	{
		public static void AddSqlParameter<T>(this IDbCommand cmd, string name, T value, SqlDbType sqlType)
		{
			var @params = cmd.Parameters;
			if (@params == null)
			{
				// Case of mock in unit testing.

				return;
			}

			if (@params is SqlParameterCollection sqlParams)
			{
				sqlParams.Add(name, sqlType).Value = value;

				return;
			}

			throw new InvalidOperationException("This extension method is only intended for sql server.");
		}
	}
}