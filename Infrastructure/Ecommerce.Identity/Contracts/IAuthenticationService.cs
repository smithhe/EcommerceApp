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
		/// <param name="request">The User to log out</param>
		Task LogoutAsync(AuthenticatedUserModel request);
		
		/// <summary>
		/// Creates a new user from the info provided in the request
		/// </summary>
		/// <param name="createUserRequest">The information of the new User to register</param>
		/// <returns>
		///	<c>True</c> if the user does not exist and the information provided is valid;
		/// <c>False</c> if any property in the request is empty or null, if the user exists, or if the information provided is invalid
		/// </returns>
		Task<bool> CreateUserAsync(CreateUserRequest createUserRequest);
	}
}