using LinkIT.Data.DTO;
using LinkIT.Data.Extensions;
using LinkIT.Data.Queries;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LinkIT.Data.Repositories
{
	public class UserRoleRepository : IUserRoleRepository
	{
		public const string ID_COLUMN = "Id";
		public const string USER_NAME_COLUMN = "UserName";
		public const string ROLES_COLUMN = "Roles";

		private readonly ConnectionString _connectionString;

		public UserRoleRepository(ConnectionString connString) =>
			_connectionString = connString ?? throw new ArgumentNullException(nameof(connString));

		private static string CreateSelectStatement() =>
			$"SELECT * FROM [{TableNames.USER_ROLE_TABLE}]";

		private UserRolesDto ReadDtoFrom(SqlDataReader reader)
		{
			var data = new Dictionary<string, IEnumerable<string>>();
			while (reader.Read())
			{
				long id = Repository<Dto, Query>.GetColumnValue<long>(reader, ID_COLUMN);
				string user = Repository<Dto, Query>.GetColumnValue<string>(reader, USER_NAME_COLUMN);
				string roles = Repository<Dto, Query>.GetColumnValue<string>(reader, ROLES_COLUMN);

				if (string.IsNullOrWhiteSpace(roles))
					throw new InvalidOperationException($"Roles not specified for record with id : '{id}'.");

				data[user] = roles.SplitCommaSeparated();
			}

			return new UserRolesDto(data);
		}

		public UserRolesDto GetAll()
		{
			using (var con = new SqlConnection(_connectionString.Value))
			{
				con.Open();
				using (var tx = con.BeginTransaction())
				{
					using (var cmd = new SqlCommand(
						CreateSelectStatement(),
						con,
						tx))
					using (var reader = cmd.ExecuteReader())
					{
						return ReadDtoFrom(reader);
					}

					//tx.Commit();
				}
			}
		}
	}
}