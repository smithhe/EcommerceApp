using Ecommerce.Identity.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ecommerce.FastEndpoints
{
	public static class TokenService
	{
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