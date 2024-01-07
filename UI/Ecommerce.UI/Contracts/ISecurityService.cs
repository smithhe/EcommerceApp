using Ecommerce.Shared.Security;
using System.Threading.Tasks;

namespace Ecommerce.UI.Contracts
{
	public interface ISecurityService
	{
		Task<CreateUserResponse> RegisterUser(CreateUserRequest createUserRequest);
		Task<bool> Login(AuthenticationRequest authenticationRequest);
		Task<bool> Logout(string userName);
		Task<UpdateEcommerceUserResponse> UpdateUser(UpdateEcommerceUserRequest updateUserRequest);
		Task<UpdatePasswordResponse> UpdatePassword(string username, string currentPassword, string newPassword);
	}
}