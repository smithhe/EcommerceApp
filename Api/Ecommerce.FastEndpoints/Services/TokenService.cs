using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.Identity.Contracts;

namespace Ecommerce.FastEndpoints.Services
{
	/// <summary>
	/// A service class that handles operations on the JWT token passed in HTTP requests
	/// </summary>
	public class TokenService : ITokenService
	{
		private readonly IAuthenticationService _authenticationService;

		public TokenService(IAuthenticationService authenticationService)
		{
			this._authenticationService = authenticationService;
		}
		
		/// <summary>
		/// Validates if the JWT token passed in the HTTP request is still valid for use
		/// </summary>
		/// <param name="token">The JWT token passed in the HTTP request</param>
		/// <returns></returns>
		public async Task<bool> ValidateTokenAsync(string? token)
		{
			if (string.IsNullOrEmpty(token))
			{
				return false;
			}
			
			//If we dont have a token no need to validate
			if (token.StartsWith("Bearer") == false)
			{
				return false;
			}
			
			token = token.Substring("Bearer ".Length);
			
			//Use the additional custom check to validate more than just the token signature
			if (await this._authenticationService.IsValidToken(token) == false)
			{
				//Token is invalid
				return false;
			}

			//Token is valid
			return true;
		}

		/// <summary>
		/// Retrieves the username from the token
		/// </summary>
		/// <param name="token">The JWT token passed in the HTTP request</param>
		/// <returns>
		/// A string containing the username if found;
		/// <c>null</c> if token is null or no claim is found for Name
		/// </returns>
		public string? GetUserNameFromToken(string? token)
		{
			//Check for null token
			if (string.IsNullOrEmpty(token))
			{
				return null;
			}
			
			token = token.Substring("Bearer ".Length);
			
			// Decode the JWT token
			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
			JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);
			
			// Find the username claim
			Claim? usernameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

			return usernameClaim?.Value;
		}

		/// <summary>
		/// Retrieves the user id from the auth token
		/// </summary>
		/// <param name="token">The auth token to parse</param>
		/// <returns>
		/// The user id if found;
		/// <c>null</c> if token is null or no claim is found for NameIdentifier
		/// </returns>
		public Guid? GetUserIdFromToken(string? token)
		{
			//Check for null token
			if (string.IsNullOrEmpty(token))
			{
				return null;
			}
			
			token = token.Substring("Bearer ".Length);
			
			// Decode the JWT token
			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
			JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);
			
			Claim? userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

			return userIdClaim == null ? null : Guid.Parse(userIdClaim.Value);
		}
	}
}