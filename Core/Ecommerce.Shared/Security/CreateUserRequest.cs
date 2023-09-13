using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.Security
{
	/// <summary>
	/// A Api request to create a new User
	/// </summary>
	public class CreateUserRequest
	{
		/// <summary>
		/// The UserName of the User to create
		/// </summary>
		[Required]
		public string? UserName { get; set; }
		
		/// <summary>
		/// The First Name of the User to create
		/// </summary>
		[Required]
		public string? FirstName { get; set; }
		
		/// <summary>
		/// The Last Name of the User to create
		/// </summary>
		[Required]
		public string? LastName { get; set; }
		
		/// <summary>
		/// The Email Address of the User to create
		/// </summary>
		[Required]
		public string? EmailAddress { get; set; }
		
		/// <summary>
		/// The Password of the User to create
		/// </summary>
		[Required]
		[MaxLength(50)]
		public string? Password { get; set; }
	}
}