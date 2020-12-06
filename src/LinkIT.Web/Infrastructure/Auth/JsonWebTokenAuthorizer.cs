﻿using LinkIT.Data.Repositories;
using log4net;
using System;
using System.Configuration;

namespace LinkIT.Web.Infrastructure.Auth
{
	public class JsonWebTokenAuthorizer
	{
		private readonly IUserRoleRepository _repo;
		private readonly IJsonWebTokenWrapper _jwt;
		private readonly ILog _log;

		public JsonWebTokenAuthorizer(IUserRoleRepository repo, IJsonWebTokenWrapper jwt)
		{
			_repo = repo ?? throw new ArgumentNullException(nameof(repo));
			_jwt = jwt ?? throw new ArgumentNullException(nameof(jwt));
			_log = LogManager.GetLogger(GetType());
		}

		private bool HasRequiredScope() =>
			_jwt.Scope.ToLowerInvariant().Contains(RequiredScope.ToLowerInvariant());

		public static string RequiredScope
		{
			get
			{
				string key = "jwt.required_scope";
				string value = ConfigurationManager.AppSettings[key];
				if (string.IsNullOrWhiteSpace(value))
					throw new InvalidOperationException($"'{key}' was not found in config file.");

				return value;
			}
		}

		public bool IsCurrentUserAuthorized(params string[] requiredRoles)
		{
			try
			{
				_jwt.Validate();
			}
			catch (Exception ex)
			{
				_log.Info($"Jwt validation failed : {ex.Message}");

				return false;
			}

			if (!_jwt.TryGetUserId(out string currentUser))
			{
				_log.Info("No user id found in the JWT token.");

				return false;
			}

			if (!HasRequiredScope())
			{
				_log.Info($"Scope for user : '{currentUser}' doesn't contain '{RequiredScope}'.");

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