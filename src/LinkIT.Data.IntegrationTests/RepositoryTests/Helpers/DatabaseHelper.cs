using LinkIT.Data.Repositories;
using System.Data;
using System.Data.SqlClient;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.Helpers
{
	internal class DatabaseHelper
	{
		internal DatabaseHelper() : this(ConnectionString.Get()) { }

		internal DatabaseHelper(string connString) =>
			ConnString = connString;

		private static string CreateUserRoleInsertStatement() =>
			$@"INSERT INTO [{TableNames.USER_ROLE_TABLE}] ([{UserRoleRepository.USER_NAME_COLUMN}], [{UserRoleRepository.ROLES_COLUMN}])
			VALUES
			('user1', 'select,create   ,     update,delete'),
			('user2', 'select,,  ,  update'),
			('user3', 'select');";

		internal string ConnString { get; private set; }

		internal void HardDeleteAll()
		{
			using (var con = new SqlConnection(ConnString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					string cmdText = $"DELETE FROM {TableNames.ASSET_HISTORY_TABLE};";
					cmdText += $"DELETE FROM {TableNames.ASSET_TABLE};";
					cmdText += $"DELETE FROM {TableNames.PRODUCT_TABLE};";
					cmdText += $"DELETE FROM {TableNames.SPECIAL_OWNER_TABLE};";
					cmdText += $"DELETE FROM {TableNames.USER_ROLE_TABLE};";

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

		internal void InsertTestUserRoles()
		{
			using (var con = new SqlConnection(ConnString))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = new SqlCommand(CreateUserRoleInsertStatement(), con, tx))
						cmd.ExecuteNonQuery();

					tx.Commit();
				}
			}
		}
	}
}