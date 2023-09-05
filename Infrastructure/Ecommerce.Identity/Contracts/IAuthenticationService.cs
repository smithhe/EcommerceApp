using Ecommerce.Shared.Security;
using System.Threading.Tasks;

namespace Ecommerce.Identity.Contracts
{
	/// <summary>
	/// Custom Authentication service to handle all security related functions
	/// </summary>
	public interface IAuthenticationService
	{
		/// <summary>
		/// Authenticates the user requesting to be authenticated
		/// </summary>
		/// <param name="request">The information of the user attempting to authenticate</param>
		/// <returns>
		///	A <see cref="AuthenticatedUserModel"/> if the user is found and the information matches correctly
		/// <c>null</c> if either property in the request is null or if the information provided does not match to an existing user correctly
		/// </returns>
		Task<AuthenticatedUserModel?> AuthenticateAsync(AuthenticationRequest request);
		
		/// <summary>
		/// Logs the user out on the server
		/// </summary>
		/// <param name="userName">The User to log out</param>
		Task LogoutAsync(string userName);
		
		/// <summary>
		/// Creates a new user from the info provided in the request
		/// </summary>
		/// <param name="createUserRequest">The information of the new User to register</param>
		/// <returns>
		///	A <see cref="CreateUserResponse"/> with success <c>true</c> if the user was created;
		/// false if the user failed to create with Errors populated with the errors that caused failure
		/// </returns>
		Task<CreateUserResponse> CreateUserAsync(CreateUserRequest createUserRequest);
		
		/// <summary>
		/// Validates the Jwt token provided in the request can still be used
		/// </summary>
		/// <param name="token">The Jwt token send in the request</param>
		/// <returns>
		/// <c>True</c> if the token is still valid
		/// <c>False</c> if the token is no longer valid
		/// </returns>
		Task<bool> IsValidToken(string token);
	}
}