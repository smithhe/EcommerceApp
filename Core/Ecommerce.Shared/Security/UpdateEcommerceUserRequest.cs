namespace Ecommerce.Shared.Security
{
	/// <summary>
	/// A Api request to update a User
	/// </summary>
	public class UpdateEcommerceUserRequest
	{
		/// <summary>
		/// The UserName of the User to update
		/// </summary>
		public string? UserName { get; set; }
		
		/// <summary>
		/// The Updated Username to use
		/// </summary>
		public string? UpdateUserName { get; set; }
		
		/// <summary>
		/// The First Name of the User to update
		/// </summary>
		public string? FirstName { get; set; }
		
		/// <summary>
		/// The Last Name of the User to update
		/// </summary>
		public string? LastName { get; set; }
		
		/// <summary>
		/// The Email of the User to update
		/// </summary>
		public string? Email { get; set; }
		
	}
}