namespace LinkIT.Web.Infrastructure.Auth
{
	public interface IJsonWebTokenWrapper
	{
		string Scope { get; }

		string Name { get; }

		string UserId { get; }

		string GivenName { get; }

		string FamilyName { get; }

		string Email { get; }

		void Validate();

		bool TryGetUserId(out string userId);
	}
}