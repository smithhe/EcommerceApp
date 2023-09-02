using System.Collections.Generic;

namespace Ecommerce.Shared.Responses
{
	/// <summary>
	/// Abstract class representing the base structure of all response objects
	/// </summary>
	public abstract class BaseResponse
	{
		/// <summary>
		/// Indicates whether the request was successful or not 
		/// </summary>
		public bool Success { get; set; }
		
		/// <summary>
		/// A optional message to display to the user
		/// </summary>
		public string? Message { get; set; }
		
		/// <summary>
		/// A list of errors that occured during validation to present to the user
		/// </summary>
		public List<string> ValidationErrors { get; set; } = new List<string>();
	}
}