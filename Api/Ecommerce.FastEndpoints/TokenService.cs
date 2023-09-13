using Ecommerce.Identity.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ecommerce.FastEndpoints
{
	/// <summary>
	/// A static service class that handles operations on the JWT token passed in HTTP requests
	/// </summary>
	public static class TokenService
	{
		/// <summary>
		/// Validates if the JWT token passed in the HTTP request is still valid for use
		/// </summary>
		/// <param name="authenticationService">The <see cref="IAuthenticationService"/> instance that handles verifying if a token is valid</param>
		/// <param name="token">The JWT token passed in the HTTP request</param>
		/// <returns></returns>
		public static async Task<bool> ValidateTokenAsync(IAuthenticationService authenticationService, string? token)
		{
			if (string.IsNullOrEmpty(token))
			{
				return false;
			}
			
			//If we dont have a token no need to validate
			if (string.IsNullOrEmpty(token) == false && token.StartsWith("Bearer"))
			{
				token = token.Substring("Bearer ".Length);

				//Use the additional custom check to validate more than just the token signature 
				if (await authenticationService.IsValidToken(token) == false)
				{
					//Token is invalid
					return false;
				}
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
		public static string? GetUserNameFromToken(string? token)
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
	}
}