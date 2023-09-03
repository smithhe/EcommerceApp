using Ecommerce.Shared.Security;
using System.Threading.Tasks;

namespace Ecommerce.Identity.Contracts
{
	public interface IAuthenticationService
	{
		Task<AuthenticatedUserModel?> AuthenticateAsync(AuthenticationRequest request);
		Task<bool> CreateUserAsync(CreateUserRequest createUserRequest);
	}
}