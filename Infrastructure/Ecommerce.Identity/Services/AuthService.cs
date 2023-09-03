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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Identity.Services
{
	public class AuthService : IAuthenticationService
	{
		private readonly EcommerceIdentityDbContext _context;
		private readonly UserManager<EcommerceUser> _userManager;
		private readonly JwtSettings _jwtSettings;

		public AuthService(EcommerceIdentityDbContext context, UserManager<EcommerceUser> userManager, IOptions<JwtSettings> jwtSettings)
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
		/// <param name="request">The User to log out</param>
		public async Task LogoutAsync(AuthenticatedUserModel request)
		{
			EcommerceUser? user = await this._userManager.FindByNameAsync(request.UserName);

			if (user != null)
			{
				// Invalidate the user's token by updating the security stamp
				await this._userManager.UpdateSecurityStampAsync(user);
			}
		}

		/// <summary>
		/// Creates a new user from the info provided in the request
		/// </summary>
		/// <param name="createUserRequest">The information of the new User to register</param>
		/// <returns>
		///	<c>True</c> if the user does not exist and the information provided is valid;
		/// <c>False</c> if any property in the request is empty or null, if the user exists, or if the information provided is invalid
		/// </returns>
		public async Task<bool> CreateUserAsync(CreateUserRequest createUserRequest)
		{
			//Check for null or empty properties in the request
			if (string.IsNullOrEmpty(createUserRequest.UserName) || string.IsNullOrEmpty(createUserRequest.Password)
			    || string.IsNullOrEmpty(createUserRequest.EmailAddress) || string.IsNullOrEmpty(createUserRequest.FirstName)
			    || string.IsNullOrEmpty(createUserRequest.LastName))
			{
				return false;
			}
			
			//Check for an existing user
			EcommerceUser? existingUser = await this._userManager.FindByNameAsync(createUserRequest.UserName);
			if (existingUser != null)
			{
				return false;
			}

			//Create the new domain user
			EcommerceUser newUser = new EcommerceUser
			{
				FirstName = createUserRequest.FirstName,
				LastName = createUserRequest.LastName,
				UserName = createUserRequest.UserName,
				Email = createUserRequest.EmailAddress,
				EmailConfirmed = true //TODO: Update to send email to confirm
			};

			//Attempt to create the user
			IdentityResult result = await this._userManager.CreateAsync(newUser, createUserRequest.Password);

			//Return whether the create succeeded or failed
			return result.Succeeded;
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
			EcommerceUser? user = await this._userManager.FindByNameAsync(username);

			//Linq expression to grab all roles for this user
			var roles = from ur in this._context.UserRoles
				join r in this._context.Roles on ur.RoleId equals r.Id
				where ur.UserId == user.Id
				select new { ur.UserId, ur.RoleId, r.Name };

			//Add additional claims for the token
			List<Claim> claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, username),
				new Claim(ClaimTypes.NameIdentifier, user!.Id),
				new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
				new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.StartOfNextDay(5)).ToUnixTimeSeconds().ToString())
			};
			
			//Add the roles as claims
			foreach(var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role.Name));
			}

			//Create a new signing key for the token
			SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._jwtSettings.Key));
			SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

			//Create a token
			JwtSecurityToken token = new JwtSecurityToken(
				new JwtHeader(signingCredentials),
				new JwtPayload(claims)
			);

			//Write the token to the model sent back to the client
			AuthenticatedUserModel output = new AuthenticatedUserModel
			{
				AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
				UserName = username
			};

			//Return the model
			return output;
		}
	}
}