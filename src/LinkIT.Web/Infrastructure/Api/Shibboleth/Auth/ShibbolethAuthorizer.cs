using LinkIT.Data.Repositories;
using log4net;
using System;

namespace LinkIT.Web.Infrastructure.Api.Shibboleth.Auth
{
	public class ShibbolethAuthorizer
	{
		private readonly IUserRoleRepository _repo;
		private readonly ShibbolethAttributes _shibbolethAttribs;
		private readonly ILog _log;

		public ShibbolethAuthorizer(IUserRoleRepository repo, ShibbolethAttributes shibbolethAttribs)
		{
			_repo = repo ?? throw new ArgumentNullException(nameof(repo));
			_shibbolethAttribs = shibbolethAttribs ?? throw new ArgumentNullException(nameof(shibbolethAttribs));
			_log = LogManager.GetLogger(GetType());
		}

		public bool IsCurrentUserAuthorized(params string[] requiredRoles)
		{
			if (!_shibbolethAttribs.TryGetUid(out string currentUser))
			{
				_log.Info("No uid found on the Shibboleth context.");

				return false;
			}

			var userRoles = _repo.GetAll();
			if (!userRoles.HasUser(currentUser))
			{
				_log.Info($"User : '{currentUser}' not found in the database.");

				return false;
			}

			if (!userRoles.HasRole(currentUser, requiredRoles))
			{
				string joined = string.Join(",", requiredRoles);
				_log.Info($"User : '{currentUser}' doesn't have all the required roles : '{joined}'.");

				return false;
			}

			return true;
		}
	}
}