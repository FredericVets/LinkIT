using LinkIT.Data.Repositories;
using LinkIT.Web.Filters.Api;
using LinkIT.Web.Infrastructure.Auth;
using LinkIT.Web.Models.Api;
using System.Linq;
using System.Web.Http;

namespace LinkIT.Web.Controllers.Api
{
	public class UsersController : ApiController
	{
		private readonly IUserRoleRepository _repo;
		private readonly IJsonWebTokenWrapper _jwt;

		public UsersController(IUserRoleRepository repo, IJsonWebTokenWrapper jwt)
		{
			_repo = repo;
			_jwt = jwt;
		}

		[Route("api/users/current")]
		[JwtAuthorize(Roles = Constants.Roles.READ)]
		public IHttpActionResult GetCurrentUser()
		{
			// User is already authenticated and authorized for the read role.
			var userId = _jwt.UserId;
			var allUserRoles = _repo.GetAll();
			var currentUserRoles = allUserRoles.GetRolesFor(userId);

			var readModel = new UserReadModel
			{
				Scope = _jwt.Scope,
				UserId = userId,
				Name = _jwt.Name,
				GivenName = _jwt.GivenName,
				FamilyName = _jwt.FamilyName,
				Email = _jwt.Email,
				Roles = currentUserRoles.ToArray()
			};

			return Ok(readModel);
		}
	}
}