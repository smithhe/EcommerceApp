namespace Ecommerce.Shared.Security
{
	public class CreateUserRequest
	{
		public string? UserName { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? EmailAddress { get; set; }
		public string? Password { get; set; }
	}
}