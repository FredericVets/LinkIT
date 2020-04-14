using LinkIT.Data.Repositories;
using System;

namespace LinkIT.Web.Infrastructure.Api.Shibboleth.Auth
{
	public class ShibbolethAuthorizer
	{
		private readonly IUserRoleRepository _repo;

		public ShibbolethAuthorizer(IUserRoleRepository repo) =>
			_repo = repo ?? throw new ArgumentNullException(nameof(repo));

		public bool IsAuthorized(string user, params string[] requiredRoles)
		{
			var userRoles = _repo.GetAll();

			if (!userRoles.HasUser(user))
				return false;

			return userRoles.HasRole(user, requiredRoles);
		}
	}
}