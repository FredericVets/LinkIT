using LinkIT.Data.Repositories;
using LinkIT.Web.Filters.Api;
using LinkIT.Web.Infrastructure.Api.Shibboleth;
using LinkIT.Web.Models.Api;
using System.Linq;
using System.Web.Http;

namespace LinkIT.Web.Controllers.Api
{
	public class UserRolesController : ApiController
	{
		private readonly IUserRoleRepository _repo;
		private readonly ShibbolethAttributes _shibbolethAttribs;

		public UserRolesController(IUserRoleRepository repo, ShibbolethAttributes shibbolethAttribs)
		{
			_repo = repo;
			_shibbolethAttribs = shibbolethAttribs;
		}

		[Route("api/user-roles/current-user")]
		[ShibbolethAuthorize(Roles = Constants.Roles.READ)]
		public IHttpActionResult GetForCurrentUser()
		{
			// User is already authenticated and authorized for the read role.
			var uid = _shibbolethAttribs.GetUid();
			var userRoles = _repo.GetAll();
			var actualRoles = userRoles.GetRolesFor(uid);

			var readModel = new UserRoleReadModel
			{
				UserName = uid,
				Roles = actualRoles.ToArray()
			};

			return Ok(readModel);
		}
	}
}