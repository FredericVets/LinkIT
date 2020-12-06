namespace LinkIT.Web.Models.Api
{
	public class UserReadModel
	{
		public string Scope { get; set; }

		public string UserId { get; set; }

		public string Name { get; set; }

		public string GivenName { get; set; }

		public string FamilyName { get; set; }

		public string Email { get; set; }

		public string[] Roles { get; set; }
	}
}