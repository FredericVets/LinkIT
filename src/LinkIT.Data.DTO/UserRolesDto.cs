using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIT.Data.DTO
{
	public class UserRolesDto
	{
		private readonly IDictionary<string, IEnumerable<string>> _data;

		public UserRolesDto(IDictionary<string, IEnumerable<string>> data) =>
			_data = data ?? throw new ArgumentNullException(nameof(data));

		private void GuardUser(string user)
		{
			if (!HasUser(user))
				throw new ArgumentException($"User '{user}' not found.");
		}

		private bool InnerTryGetRolesFor(string user, out IEnumerable<string> roles)
		{
			roles = _data[user.ToLower()];

			return roles != null && roles.Any();
		}

		public bool HasUser(string user)
		{
			if (string.IsNullOrWhiteSpace(user))
				return false;

			return _data.ContainsKey(user.ToLower());
		}

		public bool TryGetRolesFor(string user, out IEnumerable<string> roles)
		{
			roles = Enumerable.Empty<string>();

			if (!HasUser(user))
				return false;

			return InnerTryGetRolesFor(user, out roles);
		}

		public IEnumerable<string> GetRolesFor(string user)
		{
			GuardUser(user);

			if (!InnerTryGetRolesFor(user, out var roles))
				throw new ArgumentException($"No roles found for user '{user}'.");

			return roles;
		}

		public bool HasRole(string user, params string[] roles)
		{
			GuardUser(user);

			if (roles == null || !roles.Any())
				throw new ArgumentNullException(nameof(roles));

			if (!InnerTryGetRolesFor(user, out var actualRoles))
				return false;

			return roles.All(r => actualRoles.Contains(r, StringComparer.InvariantCultureIgnoreCase));
		}
	}
}