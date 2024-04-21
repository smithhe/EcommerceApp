using System.Net;
using Blazored.LocalStorage;
using Ecommerce.UI.Contracts;
using Ecommerce.UI.Contracts.Refit;
using Ecommerce.UI.Security;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Refit;
using System.Threading.Tasks;
using Ecommerce.Shared.Security;
using Ecommerce.Shared.Security.Requests;
using Ecommerce.Shared.Security.Responses;

namespace Ecommerce.UI.Services
{
	public class SecurityService : ISecurityService
	{
		private readonly ISecurityApiService _securityApiService;
		private readonly ILocalStorageService _localStorageService;
		private readonly AuthenticationStateProvider _authenticationStateProvider;
		private readonly string _authTokenStorageKey;
		
		public SecurityService(ISecurityApiService securityApiService, ILocalStorageService localStorageService,
			IConfiguration configuration, AuthenticationStateProvider authenticationStateProvider)
		{
			this._securityApiService = securityApiService;
			this._localStorageService = localStorageService;
			this._authenticationStateProvider = authenticationStateProvider;
			this._authTokenStorageKey = configuration["authTokenStorageKey"]!;
		}
		
		public async Task<CreateUserResponse> RegisterUser(CreateUserRequest createUserRequest)
		{
			ApiResponse<CreateUserResponse> response = await this._securityApiService.CreateUser(createUserRequest);

			if (response.IsSuccessStatusCode)
			{
				return response.Content;
			}
			
			return string.IsNullOrEmpty(response.Error.Content) ? 
				new CreateUserResponse { Success = false, Errors = new []{ "Unexpected Error Occurred" } } 
				: JsonConvert.DeserializeObject<CreateUserResponse>(response.Error.Content)!;
		}

		public async Task<LoginResponse> Login(AuthenticationRequest authenticationRequest)
		{
			ApiResponse<AuthenticateResponse> apiResponse = await this._securityApiService.Login(authenticationRequest);

			if (apiResponse.IsSuccessStatusCode == false)
			{
				if (apiResponse.StatusCode == HttpStatusCode.Unauthorized)
				{
					return new LoginResponse { IsSuccessful = false, Message = "Invalid Credentials" };
				}

				if (apiResponse.StatusCode == HttpStatusCode.Forbidden)
				{
					return new LoginResponse { IsSuccessful = false, Message = "Account is Locked or Disabled" };
				}
			}

			AuthenticateResponse? response = apiResponse.Content;

			if (response == null)
			{
				return new LoginResponse { IsSuccessful = false, Message = "Unexpected Error Occurred" };
			}
			
			if (response.SignInResult == SignInResponseResult.TwoFactorRequired)
			{
				//TODO: Implement Two Factor Authentication
				return new LoginResponse { IsSuccessful = false, Message = "Two Factor Authentication Required" };
			}

			if (response.SignInResult == SignInResponseResult.EmailNotConfirmed)
			{
				return new LoginResponse { IsSuccessful = false, Message = "Email Confirmation Required" };
			}

			//Store the access token on browser storage
			await this._localStorageService.SetItemAsync(this._authTokenStorageKey, response.Token);
				
			((AuthStateProvider)this._authenticationStateProvider).NotifyUserAuthentication(response.Token!);

			return new LoginResponse { IsSuccessful = true };
		}

		public async Task<bool> Logout(string userName)
		{
			await this._securityApiService.Logout(new LogoutUserRequest { UserName = userName });

			await this._localStorageService.RemoveItemAsync(this._authTokenStorageKey);
			((AuthStateProvider)this._authenticationStateProvider).NotifyUserLogout();

			return true;
		}
		
		public async Task<UpdateEcommerceUserResponse> UpdateUser(UpdateEcommerceUserRequest updateUserRequest)
		{
			ApiResponse<UpdateEcommerceUserResponse> response = await this._securityApiService.UpdateUser(updateUserRequest);

			if (response.IsSuccessStatusCode == false)
			{
				if (string.IsNullOrEmpty(response.Error.Content))
				{
					return new UpdateEcommerceUserResponse { Success = false, Message = "Unexpected Error Occurred" };
				}
			
				UpdateEcommerceUserResponse? error = JsonConvert.DeserializeObject<UpdateEcommerceUserResponse>(response.Error.Content);
				return error!;
			}
			
			UpdateEcommerceUserResponse content = response.Content;
			
			if (string.IsNullOrEmpty(content.UpdatedAccessToken))
			{
				return content;
			}

			//Store the access token on browser storage
			await this._localStorageService.SetItemAsync(this._authTokenStorageKey, content.UpdatedAccessToken);
			((AuthStateProvider)this._authenticationStateProvider).NotifyUserAuthentication(content.UpdatedAccessToken);
			
			return content;
		}

		public async Task<UpdatePasswordResponse> UpdatePassword(string username, string currentPassword, string newPassword)
		{
			ApiResponse<UpdatePasswordResponse> response = await this._securityApiService.UpdatePassword(new UpdatePasswordRequest
			{
				UserName = username,
				CurrentPassword = currentPassword,
				NewPassword = newPassword
			});

			if (response.IsSuccessStatusCode == false)
			{
				if (string.IsNullOrEmpty(response.Error.Content))
				{
					return new UpdatePasswordResponse { Success = false, Message = "Unexpected Error Occurred" };
				}

				UpdatePasswordResponse? error = JsonConvert.DeserializeObject<UpdatePasswordResponse>(response.Error.Content);
				return error!;
			}
			
			UpdatePasswordResponse content = response.Content;
			
			if (string.IsNullOrEmpty(content.UpdatedAccessToken))
			{
				return content;
			}
			
			//Store the access token on browser storage
			await this._localStorageService.SetItemAsync(this._authTokenStorageKey, content.UpdatedAccessToken);
			((AuthStateProvider)this._authenticationStateProvider).NotifyUserAuthentication(content.UpdatedAccessToken);
			
			return content;
		}

		public async Task<ConfirmEmailResponse> ConfirmUserEmail(string? userId, string? emailToken)
		{
			ApiResponse<ConfirmEmailResponse?> response = await this._securityApiService.ConfirmEmail(new ConfirmEmailRequest
			{
				UserId = userId,
				EmailToken = emailToken
			});
			
			if (response.IsSuccessStatusCode)
			{
				return response.Content;
			}
			
			return string.IsNullOrEmpty(response.Error.Content) ? 
				new ConfirmEmailResponse { Success = false, Message = "Unexpected Error Occurred" } 
				: JsonConvert.DeserializeObject<ConfirmEmailResponse>(response.Error.Content)!;
		}
	}
}