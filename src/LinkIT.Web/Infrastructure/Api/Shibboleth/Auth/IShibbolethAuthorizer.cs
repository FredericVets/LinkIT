namespace LinkIT.Web.Infrastructure.Api.Shibboleth.Auth
{
	public interface IShibbolethAuthorizer
	{
		bool IsAuthorized(string user, params string[] requiredRoles);
	}
}