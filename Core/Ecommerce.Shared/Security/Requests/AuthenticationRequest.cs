namespace Ecommerce.Shared.Security.Requests
{
	/// <summary>
	/// The Api Request to authenticate a user based on UserName and Password
	/// </summary>
	public class AuthenticationRequest
	{
		/// <summary>
		/// The UserName of the user attempting to authenticate
		/// </summary>
		public string? UserName { get; set; }
		
		/// <summary>
		/// The Password of the user attempting to authenticate
		/// </summary>
		public string? Password { get; set; }
	}
}