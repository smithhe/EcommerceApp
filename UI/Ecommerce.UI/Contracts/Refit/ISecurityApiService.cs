using Ecommerce.Shared.Security;
using Refit;
using System.Threading.Tasks;
using Ecommerce.Shared.Security.Requests;
using Ecommerce.Shared.Security.Responses;

namespace Ecommerce.UI.Contracts.Refit
{
	public interface ISecurityApiService
	{
		[Post("/api/register")]
		Task<ApiResponse<CreateUserResponse>> CreateUser(CreateUserRequest createUserRequest);
		
		[Post("/api/login")]
		Task<ApiResponse<AuthenticatedUserModel?>> Login(AuthenticationRequest authenticationRequest);
		
		[Post("/api/logout")]
		Task<IApiResponse> Logout(LogoutUserRequest logoutRequest);
		
		[Put("/api/user/update")]
		Task<ApiResponse<UpdateEcommerceUserResponse>> UpdateUser(UpdateEcommerceUserRequest updateUserRequest);
		
		[Put("/api/password/update")]
		Task<ApiResponse<UpdatePasswordResponse>> UpdatePassword(UpdatePasswordRequest updatePasswordRequest);
		
		[Post("/api/user/confirm-email")]
		Task<ApiResponse<ConfirmEmailResponse?>> ConfirmEmail(ConfirmEmailRequest confirmEmailRequest);
	}
}