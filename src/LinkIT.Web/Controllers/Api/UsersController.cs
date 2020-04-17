using LinkIT.Data.Repositories;
using LinkIT.Web.Filters.Api;
using LinkIT.Web.Infrastructure.Shibboleth;
using LinkIT.Web.Models.Api;
using System.Linq;
using System.Web.Http;

namespace LinkIT.Web.Controllers.Api
{
	public class UsersController : ApiController
	{
		private readonly IUserRoleRepository _repo;
		private readonly ShibbolethAttributes _shibbolethAttribs;

		public UsersController(IUserRoleRepository repo, ShibbolethAttributes shibbolethAttribs)
		{
			_repo = repo;
			_shibbolethAttribs = shibbolethAttribs;
		}

		[Route("api/users/current")]
		[ShibbolethAuthorize(Roles = Constants.Roles.READ)]
		public IHttpActionResult GetForCurrentUser()
		{
			// User is already authenticated and authorized for the read role.
			var uid = _shibbolethAttribs.UId;
			var userRoles = _repo.GetAll();
			var actualRoles = userRoles.GetRolesFor(uid);

			var readModel = new UserReadModel
			{
				UserName = uid,
				SurName = _shibbolethAttribs.SurName,
				GivenName = _shibbolethAttribs.GivenName,
				Email = _shibbolethAttribs.Email,
				Roles = actualRoles.ToArray()
			};

			return Ok(readModel);
		}
	}
}