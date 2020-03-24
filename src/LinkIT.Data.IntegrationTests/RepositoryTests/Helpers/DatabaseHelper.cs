using System.Data;
using System.Data.SqlClient;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.Helpers
{
	internal class DatabaseHelper
	{
		internal DatabaseHelper() : this(ConnectionString.Get()) { }

		internal DatabaseHelper(string connString) =>
			ConnString = connString;

		internal string ConnString { get; private set; }

		internal void HardDeleteAll()
		{
			using (var con = new SqlConnection(ConnString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText = "DELETE FROM AssetHistory;";
					cmdText += "DELETE FROM Asset;";
					cmdText += "DELETE FROM Product;";
					cmdText += "DELETE FROM SpecialOwner;";

					using (var cmd = new SqlCommand(cmdText, con, tx))
						cmd.ExecuteNonQuery();

					tx.Commit();
				}
			}
		}

		internal bool IsAssetMarkedAsDeleted(long id)
		{
			using (var con = new SqlConnection(ConnString))
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
	}
}