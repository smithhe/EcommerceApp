using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.Requests.EcommerceUser
{
	/// <summary>
	/// A Api request to update a User
	/// </summary>
	public class UpdateEcommerceUserApiRequest
	{
		/// <summary>
		/// The UserName of the User to update
		/// </summary>
		public string? UserName { get; set; }
		
		/// <summary>
		/// The First Name of the User to update
		/// </summary>
		public string? FirstName { get; set; }
		
		/// <summary>
		/// The Last Name of the User to update
		/// </summary>
		public string? LastName { get; set; }
		
		/// <summary>
		/// The Email Address of the User to update
		/// </summary>
		public string? EmailAddress { get; set; }
	}
}