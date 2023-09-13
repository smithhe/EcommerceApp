using System.Collections.Generic;

namespace Ecommerce.Shared.Security
{
	/// <summary>
	/// A response object for creating a new User
	/// </summary>
	public class CreateUserResponse
	{
		/// <summary>
		/// Indicates whether the request was successful or not 
		/// </summary>
		public bool Success { get; set; }
		
		/// <summary>
		/// Holds all errors that occurred during User registration
		/// </summary>
		public IEnumerable<string>? Errors { get; set; }
	}
}