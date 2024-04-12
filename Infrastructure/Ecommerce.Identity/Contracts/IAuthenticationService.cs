using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Security;
using System;
using System.Threading.Tasks;
using Ecommerce.Shared.Security.Requests;
using Ecommerce.Shared.Security.Responses;

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
		Task<AuthenticateResponse> AuthenticateAsync(AuthenticationRequest request);
		
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
		/// Attempts to confirm the email of the user with the provided token
		/// </summary>
		/// <param name="userId">The unique identifier of the user</param>
		/// <param name="token">The token used to confirm the email address</param>
		/// <returns>
		/// Returns <c>true</c> if the email was confirmed;
		/// Returns <c>false</c> if the user was not found or the token was invalid
		/// </returns>
		Task<ConfirmEmailResponse> ConfirmEmailAsync(string userId, string token);
		
		/// <summary>
		/// Validates the Jwt token provided in the request can still be used
		/// </summary>
		/// <param name="token">The Jwt token send in the request</param>
		/// <returns>
		/// <c>True</c> if the token is still valid
		/// <c>False</c> if the token is no longer valid
		/// </returns>
		Task<bool> IsValidToken(string token);

		/// <summary>
		/// Updates the information for an existing <see cref="EcommerceUser"/>
		/// </summary>
		/// <param name="user">The <see cref="EcommerceUser"/> to update with</param>
		/// <param name="username">The username to update the <see cref="EcommerceUser"/> with</param>
		/// <returns>
		/// A <see cref="UpdateEcommerceUserResponse"/> with success <c>true</c> if the user was updated;
		/// false if the user failed to update with ValidationErrors populated with the errors that caused failure
		/// </returns>
		Task<UpdateEcommerceUserResponse> UpdateUser(EcommerceUser? user, string username);

		/// <summary>
		/// Updates the password for an existing <see cref="EcommerceUser"/>
		/// </summary>
		/// <param name="user">The <see cref="EcommerceUser"/> to update the password for</param>
		/// <param name="currentPassword">The current password of the User</param>
		/// <param name="newPassword">The new password to update to</param>
		/// <returns>
		/// A <see cref="UpdatePasswordResponse"/> with success <c>true</c> if the password was updated;
		/// false if the password failed to update with ValidationErrors populated with the errors that caused failure
		/// </returns>
		Task<UpdatePasswordResponse> UpdatePassword(EcommerceUser? user, string currentPassword, string newPassword);

		/// <summary>
		/// Retrieves a <see cref="EcommerceUser"/> if any exist
		/// </summary>
		/// <param name="id">The unique identifier of the User to find</param>
		/// <returns>
		/// A <see cref="EcommerceUser"/>;
		/// <c>null</c> if no User is found
		/// </returns>
		Task<EcommerceUser?> GetUserById(Guid id);

		/// <summary>
		/// Retrieves the unique identifier of the <see cref="EcommerceUser"/> if it exists
		/// </summary>
		/// <param name="userName">The username of the User</param>
		/// <returns>
		/// The unique identifier of the <see cref="EcommerceUser"/>;
		/// null if no User exists with the provided username
		/// </returns>
		Task<Guid?> GetUserIdByName(string userName);
	}
}