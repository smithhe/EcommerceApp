using Ecommerce.Identity.Contracts;
using System.Threading.Tasks;

namespace Ecommerce.FastEndpoints
{
	public static class TokenValidatorService
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
	}
}