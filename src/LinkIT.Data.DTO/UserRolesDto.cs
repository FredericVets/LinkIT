using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIT.Data.DTO
{
	public class UserRolesDto
	{
		public UserRolesDto(IDictionary<string, IEnumerable<string>> data) =>
			Data = data ?? throw new ArgumentNullException(nameof(data));

		private void GuardUser(string user)
		{
			if (!HasUser(user))
				throw new ArgumentException($"User '{user}' not found.");
		}

		public IDictionary<string, IEnumerable<string>> Data { get; }

		public bool HasUser(string user)
		{
			if (string.IsNullOrWhiteSpace(user))
				throw new ArgumentNullException(nameof(user));

			return Data.ContainsKey(user.ToLower());
		}

		public bool TryGetRolesFor(string user, out IEnumerable<string> roles)
		{
			roles = Enumerable.Empty<string>();

			if (!HasUser(user))
				return false;

			roles = Data[user.ToLower()];
			if (roles == null || !roles.Any())
				return false;

			return true;
		}

		public IEnumerable<string> GetRolesFor(string user)
		{
			GuardUser(user);

			if (!TryGetRolesFor(user, out var roles))
				throw new ArgumentException($"No roles found for user '{user}'.");

			return roles;
		}

		public bool HasRole(string user, params string[] roles)
		{
			GuardUser(user);

			if (roles == null || !roles.Any())
				throw new ArgumentNullException(nameof(roles));

			if (!TryGetRolesFor(user, out var actualRoles))
				return false;

			return roles.All(r => actualRoles.Contains(r, StringComparer.InvariantCultureIgnoreCase));
		}
	}
}