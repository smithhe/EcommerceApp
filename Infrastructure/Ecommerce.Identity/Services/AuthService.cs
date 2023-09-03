using Ecommerce.Identity.Contracts;
using Ecommerce.Shared.Security;
using System.Threading.Tasks;

namespace Ecommerce.Identity.Services
{
	public class AuthService : IAuthenticationService
	{
		public async Task<AuthenticatedUserModel?> AuthenticateAsync(AuthenticationRequest request)
		{
			throw new System.NotImplementedException();
		}

		public async Task<bool> CreateUserAsync(CreateUserRequest createUserRequest)
		{
			throw new System.NotImplementedException();
		}
	}
}