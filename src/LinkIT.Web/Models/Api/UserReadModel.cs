namespace LinkIT.Web.Models.Api
{
	public class UserReadModel
	{
		public string UserName { get; set; }

		public string GivenName { get; set; }

		public string SurName { get; set; }

		public string Email { get; set; }

		public string[] Roles { get; set; }
	}
}