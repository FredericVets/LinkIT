using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.AssetRepo
{
	internal static class AssetDatabaseHelper
	{
		internal static string ConnectionString
		{
			get => ConfigurationManager.ConnectionStrings["LinkITConnectionString"].ConnectionString;
		}

		public static bool IsMarkedAsDeleted(long id)
		{
			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText = "SELECT Deleted FROM ASSET WHERE Id = @Id";

					using (var cmd = new SqlCommand(cmdText, con, tx))
					{
						cmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = id;
						bool deleted = (bool)cmd.ExecuteScalar();

						return deleted;
					}
				}
			}
		}

		public static void HardDelete(long id)
		{
			using (var con = new SqlConnection(ConnectionString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText = "DELETE FROM ASSET WHERE Id = @Id";

					using (var cmd = new SqlCommand(cmdText, con, tx))
					{
						cmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = id;
						cmd.ExecuteNonQuery();
					}

					tx.Commit();
				}
			}
		}
	}
}