using System.Threading.Tasks;
using Ecommerce.Shared.Security.Requests;
using Ecommerce.Shared.Security.Responses;
using Ecommerce.UI.Security;

namespace Ecommerce.UI.Contracts
{
	public interface ISecurityService
	{
		Task<CreateUserResponse> RegisterUser(CreateUserRequest createUserRequest);
		Task<LoginResponse> Login(AuthenticationRequest authenticationRequest);
		Task<bool> Logout(string userName);
		Task<UpdateEcommerceUserResponse> UpdateUser(UpdateEcommerceUserRequest updateUserRequest);
		Task<UpdatePasswordResponse> UpdatePassword(string username, string currentPassword, string newPassword);
		Task<ConfirmEmailResponse> ConfirmUserEmail(string? userId, string? emailToken);
	}
}