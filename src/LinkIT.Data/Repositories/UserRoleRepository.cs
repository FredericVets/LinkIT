using LinkIT.Data.DTO;
using LinkIT.Data.Queries;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace LinkIT.Data.Repositories
{
	public class UserRoleRepository : IUserRoleRepository
	{
		public const string ID_COLUMN = "Id";
		public const string USER_NAME_COLUMN = "UserName";
		public const string ROLES_COLUMN = "Roles";

		private readonly string _connectionString;

		public UserRoleRepository(string connectionString) =>
			_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

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

				data[user] = roles
					.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(x => x.Trim())
					.Except(new[] { string.Empty });
			}

			return new UserRolesDto(data);
		}

		public UserRolesDto GetAll()
		{
			using (var con = new SqlConnection(_connectionString))
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
				}
			}
		}
	}
}