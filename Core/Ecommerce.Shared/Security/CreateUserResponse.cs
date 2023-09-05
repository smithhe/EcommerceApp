using System.Collections.Generic;

namespace Ecommerce.Shared.Security
{
	public class CreateUserResponse
	{
		public bool Success { get; set; }
		public IEnumerable<string>? Errors { get; set; }
	}
}