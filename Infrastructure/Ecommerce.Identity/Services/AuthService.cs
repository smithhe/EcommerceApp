using Ecommerce.Domain.Entities;
using Ecommerce.Identity.Contracts;
using Ecommerce.Identity.Models;
using Ecommerce.Shared.Extensions;
using Ecommerce.Shared.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Persistence;
using Ecommerce.Shared.Security.Requests;
using Ecommerce.Shared.Security.Responses;

namespace Ecommerce.Identity.Services
{
	public class AuthService : IAuthenticationService
	{
		private readonly EcommercePersistenceDbContext _context;
		private readonly UserManager<EcommerceUser> _userManager;
		private readonly JwtSettings _jwtSettings;

		public AuthService(EcommercePersistenceDbContext context, UserManager<EcommerceUser> userManager, IOptions<JwtSettings> jwtSettings)
		{
			this._context = context;
			this._userManager = userManager;
			this._jwtSettings = jwtSettings.Value;
		}
		
		/// <summary>
		/// Authenticates the user requesting to be authenticated
		/// </summary>
		/// <param name="request">The information of the user attempting to authenticate</param>
		/// <returns>
		///	A <see cref="AuthenticatedUserModel"/> if the user is found and the information matches correctly
		/// <c>null</c> if either property in the request is null or if the information provided does not match to an existing user correctly
		/// </returns>
		public async Task<AuthenticatedUserModel?> AuthenticateAsync(AuthenticationRequest request)
		{
			//Check if either property is null in the request
			if (request.UserName == null || request.Password == null)
			{
				return null;
			}
			
			//Check if the username and password are valid
			if (await IsValidUsernameAndPassword(request.UserName, request.Password))
			{
				return await GenerateToken(request.UserName);
			}

			return null;
		}

		/// <summary>
		/// Logs the user out on the server
		/// </summary>
		/// <param name="userName">The User to log out</param>
		public async Task LogoutAsync(string userName)
		{
			EcommerceUser? user = await this._userManager.FindByNameAsync(userName);

			if (user != null)
			{
				// Invalidate the user's token by updating the security stamp
				await this._userManager.UpdateSecurityStampAsync(user);
			}
		}
		
		/// <summary>
		/// Validates the Jwt token provided in the request can still be used
		/// </summary>
		/// <param name="token">The Jwt token send in the request</param>
		/// <returns>
		/// <c>True</c> if the token is still valid
		/// <c>False</c> if the token is no longer valid
		/// </returns>
		public async Task<bool> IsValidToken(string token)
		{
			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
			
			//Same as the parameters that are setup in Service Registration
			TokenValidationParameters validationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				RequireExpirationTime = true,
				ClockSkew = TimeSpan.FromMinutes(5),
				ValidIssuer = this._jwtSettings.Issuer,
				ValidAudience = this._jwtSettings.Audience,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._jwtSettings.Key))
			};

			try
			{
				ClaimsPrincipal? claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken _);
				
				// Extract the username claim from the token's claims
				string? usernameClaim = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;

				if (string.IsNullOrEmpty(usernameClaim))
				{
					// Username claim is missing or invalid
					return false;
				}
				
				// Retrieve the user's security stamp
				EcommerceUser? user = await this._userManager.FindByNameAsync(usernameClaim);
				string? userSecurityStamp = user?.SecurityStamp;

				string? securityStampClaim = claimsPrincipal.Claims.FirstOrDefault(claim => string.Equals(claim.Type, CustomClaims._securityStamp) )?.Value;

				// Check if the token's security stamp matches the stored security stamp
				if (string.IsNullOrEmpty(userSecurityStamp) == false &&
				    string.Equals(securityStampClaim, userSecurityStamp) == false)
				{
					return false; // Token is invalidated due to logout
				}

				// Check other validation conditions if needed
				//TODO: Add additional checks on the token

				return true; // Token is valid
			}
			catch (Exception)
			{
				// Token validation failed
				return false;
			}
		}

		/// <summary>
		/// Creates a new user from the info provided in the request
		/// </summary>
		/// <param name="createUserRequest">The information of the new User to register</param>
		/// <returns>
		///	A <see cref="CreateUserResponse"/> with success <c>true</c> if the user was created;
		/// false if the user failed to create with Errors populated with the errors that caused failure
		/// </returns>
		public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest createUserRequest)
		{
			CreateUserResponse response = new CreateUserResponse();
			
			//Check for null or empty properties in the request
			if (string.IsNullOrEmpty(createUserRequest.UserName) || string.IsNullOrEmpty(createUserRequest.Password)
			    || string.IsNullOrEmpty(createUserRequest.EmailAddress) || string.IsNullOrEmpty(createUserRequest.FirstName)
			    || string.IsNullOrEmpty(createUserRequest.LastName))
			{
				response.Success = false;
				response.Errors = new [] { "All Fields are required" };
				return response;
			}
			
			//Check for an existing user
			EcommerceUser? existingUser = await this._userManager.FindByNameAsync(createUserRequest.UserName);
			if (existingUser != null)
			{
				response.Success = false;
				response.Errors = new [] { "UserName Already Exists" };
				return response;
			}
			
			//Validate the email address provided
			try
			{
				MailAddress unused = new MailAddress(createUserRequest.EmailAddress);
			}
			catch (FormatException)
			{
				response.Success = false;
				response.Errors = new [] { "Invalid Email Address" };
				return response;
			}

			//Create the new domain user after sanitizing input
			EcommerceUser newUser = new EcommerceUser
			{
				FirstName = createUserRequest.FirstName.LowerAndUpperFirst(),
				LastName = createUserRequest.LastName.LowerAndUpperFirst(),
				UserName = createUserRequest.UserName.ToLower(),
				Email = createUserRequest.EmailAddress.ToLower(),
				EmailConfirmed = false
			};

			//Attempt to create the user
			IdentityResult result = await this._userManager.CreateAsync(newUser, createUserRequest.Password);

			//Check for errors
			if (result.Succeeded)
			{
				//Generate a confirmation token for the user and return the response
				response.Success = true;
				response.ConfirmationLink = await this._userManager.GenerateEmailConfirmationTokenAsync(newUser); //Temporarily store the token in the confirmation link property
				return response;
			}
			
			//Add errors into the list then return the response
			response.Success = false;
			response.Errors = result.Errors.Select(error => error.Description).ToArray();
			return response;
		}

		/// <summary>
		/// Attempts to confirm the email of the user with the provided token
		/// </summary>
		/// <param name="userId">The unique identifier of the user</param>
		/// <param name="token">The token used to confirm the email address</param>
		/// <returns>
		/// Returns <c>true</c> if the email was confirmed;
		/// Returns <c>false</c> if the user was not found or the token was invalid
		/// </returns>
		public async Task<ConfirmEmailResponse> ConfirmEmailAsync(string userId, string token)
		{
			//Attempt to find the user
			EcommerceUser? user = await this._userManager.FindByIdAsync(userId);

			//Check if the user was found
			if (user == null)
			{
				return new ConfirmEmailResponse { Success = false, Message = "User Not Found" };
			}

			//Attempt to confirm the email
			IdentityResult result = await this._userManager.ConfirmEmailAsync(user, token);

			//Return the response
			return result.Succeeded ? 
				new ConfirmEmailResponse { Success = true, Message = "Email Confirmed" } 
				: new ConfirmEmailResponse { Success = false, Message = "Email Confirmation Failed" };
		}
		
		/// <summary>
		/// Validates if the Username and Password belong to an existing user
		/// </summary>
		/// <param name="username">The username of the User attempting to authenticate</param>
		/// <param name="password">The password of the User attempting to authenticate</param>
		/// <returns>
		/// <c>True</c> if the username and password belong to a existing user;
		/// <c>False</c> if the user is not found or the password doesn't match
		/// </returns>
		private async Task<bool> IsValidUsernameAndPassword(string username, string password)
		{
			EcommerceUser? user = await this._userManager.FindByNameAsync(username);

			if (user == null)
			{
				return false;
			}

			return await this._userManager.CheckPasswordAsync(user, password);
		}
		
		/// <summary>
		/// Generates JWT token for the specified user
		/// </summary>
		/// <param name="username">The UserName of the User to create a Token for</param>
		/// <returns>A <see cref="AuthenticatedUserModel"/> with the generated token</returns>
		private async Task<AuthenticatedUserModel> GenerateToken(string username)
		{
			//Find the user in the database
			EcommerceUser user = (await this._userManager.FindByNameAsync(username))!;

			//Linq expression to grab all roles for this user
			var roles = from ur in this._context.UserRoles
				join r in this._context.Roles on ur.RoleId equals r.Id
				where ur.UserId == user.Id
				select new { ur.UserId, ur.RoleId, r.Name };

			//Add additional claims for the token
			List<Claim> claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, username),
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Email, user.Email!),
				new Claim(CustomClaims._firstName, user.FirstName),
				new Claim(CustomClaims._lastName, user.LastName)
			};
			
			//Add the roles as claims
			foreach(var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role.Name));
			}
			
			//Add the security stamp
			string securityStamp = await this._userManager.GetSecurityStampAsync(user);
			claims.Add(new Claim(CustomClaims._securityStamp, securityStamp));

			//Create a new signing key for the token
			SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._jwtSettings.Key));
			SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

			//Create a token
			JwtSecurityToken token = new JwtSecurityToken(
				this._jwtSettings.Issuer,
				this._jwtSettings.Audience,
				claims,
				DateTime.Now,
				DateTime.Now.StartOfNextDay(5),
				signingCredentials);

			//Write the token to the model sent back to the client
			AuthenticatedUserModel output = new AuthenticatedUserModel
			{
				AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
				UserName = username
			};

			//Return the model
			return output;
		}

		/// <summary>
		/// Updates the information for an existing <see cref="EcommerceUser"/>
		/// </summary>
		/// <param name="user">The <see cref="EcommerceUser"/> to update with</param>
		/// <param name="username">The username to update the <see cref="EcommerceUser"/> with</param>
		/// <returns>
		/// A <see cref="UpdateEcommerceUserResponse"/> with success <c>true</c> if the user was updated and has a new access token;
		/// false if the user failed to update with ValidationErrors populated with the errors that caused failure
		/// </returns>
		public async Task<UpdateEcommerceUserResponse> UpdateUser(EcommerceUser? user, string username)
		{
			UpdateEcommerceUserResponse response = new UpdateEcommerceUserResponse();
			
			//Verify a user was sent
			if (user == null)
			{
				response.Success = false;
				response.Message = "Must Give a User to Update With";
				return response;
			}
			
			//Check for the existing user
			EcommerceUser? existingUser = await this._userManager.FindByIdAsync(user.Id.ToString());
			if (existingUser == null)
			{
				response.Success = false;
				response.Message = "User Must Exist To Update";
				return response;
			}
			
			//Update the username for the user
			IdentityResult userNameUpdate = await this._userManager.SetUserNameAsync(user, username);
			if (userNameUpdate.Succeeded == false)
			{
				response.Success = false;
				response.Message = "Username Update Failed";
				return response;
			}
			
			//Update the user
			user.UserName = username;
			IdentityResult result = await this._userManager.UpdateAsync(user);
			
			//Check for errors
			if (result.Succeeded == false)
			{
				//Add errors into the list then return the response
				response.Success = false;
				response.ValidationErrors = result.Errors.Select(error => error.Description).ToList();
				return response;
			}

			//Generate a new token for the user
			AuthenticatedUserModel userModel = await this.GenerateToken(user.UserName!);

			//Check if the token was generated
			if (string.IsNullOrEmpty(userModel.AccessToken))
			{
				response.Success = false;
				response.Message = "Token Update Failed";
				return response;
			}
			
			//Return success with the new token
			response.UpdatedAccessToken = userModel.AccessToken;
			response.Success = true;
			return response;
		}

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
		public async Task<UpdatePasswordResponse> UpdatePassword(EcommerceUser? user, string currentPassword, string newPassword)
		{
			UpdatePasswordResponse response = new UpdatePasswordResponse();
			
			//Verify a user was sent
			if (user == null)
			{
				response.Success = false;
				response.Message = "Must Give a User to Update With";
				return response;
			}
			
			//Check for the existing user
			EcommerceUser? existingUser = await this._userManager.FindByIdAsync(user.Id.ToString());
			if (existingUser == null)
			{
				response.Success = false;
				response.Message = "User Must Exist To Update";
				return response;
			}
			
			//Update the password
			IdentityResult updatePasswordResult = await this._userManager.ChangePasswordAsync(existingUser, currentPassword, newPassword);

			//Check for errors
			if (updatePasswordResult.Succeeded == false)
			{
				response.Success = false;
				response.Message = "Password Update Failed";
				response.ValidationErrors = updatePasswordResult.Errors.Select(error => error.Description).ToList();
				return response;
			}
			
			//Generate a new token for the user
			AuthenticatedUserModel userModel = await this.GenerateToken(user.UserName!);

			//Check if the token was generated
			if (string.IsNullOrEmpty(userModel.AccessToken))
			{
				response.Success = false;
				response.Message = "Token Update Failed";
				return response;
			}

			//Return success
			response.Success = true;
			response.UpdatedAccessToken = userModel.AccessToken;
			response.Message = "Password Updated Successfully";
			return response;
		}
		
		/// <summary>
		/// Retrieves a <see cref="EcommerceUser"/> if any exist
		/// </summary>
		/// <param name="id">The unique identifier of the User to find</param>
		/// <returns>
		/// A <see cref="EcommerceUser"/>;
		/// <c>null</c> if no User is found
		/// </returns>
		public async Task<EcommerceUser?> GetUserById(Guid id)
		{
			return await this._userManager.FindByIdAsync(id.ToString());
		}

		/// <summary>
		/// Retrieves the unique identifier of the <see cref="EcommerceUser"/> if it exists
		/// </summary>
		/// <param name="userName">The username of the User</param>
		/// <returns>
		/// The unique identifier of the <see cref="EcommerceUser"/>;
		/// null if no User exists with the provided username
		/// </returns>
		public async Task<Guid?> GetUserIdByName(string userName)
		{
			EcommerceUser? user = await this._userManager.FindByNameAsync(userName);

			return user?.Id;
		}
		
	}
}