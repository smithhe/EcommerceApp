using Ecommerce.Shared.Security;
using System.Threading.Tasks;
using Ecommerce.Shared.Security.Requests;
using Ecommerce.Shared.Security.Responses;

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