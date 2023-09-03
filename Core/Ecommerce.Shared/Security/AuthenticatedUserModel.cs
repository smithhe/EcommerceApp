namespace Ecommerce.Shared.Security
{
	/// <summary>
	/// Class used to model an authenticated user in the Ecommerce App
	/// </summary>
	public class AuthenticatedUserModel
	{
		/// <summary>
		/// The token used to authenticate requests for the user
		/// </summary>
		public string AccessToken { get; set; } = null!;
		
		/// <summary>
		/// The UserName of the authenticated user
		/// </summary>
		public string UserName { get; set; } = null!;
	}
}