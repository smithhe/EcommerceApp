using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ecommerce.UI.Security
{
	public class AuthStateProvider : AuthenticationStateProvider
	{
		private readonly ILocalStorageService _localStorageService;
		private readonly IConfiguration _configuration;
		private readonly AuthenticationState _anonymous;

		public AuthStateProvider(ILocalStorageService localStorageService, IConfiguration configuration
			)
		{
			this._localStorageService = localStorageService;
			this._configuration = configuration;
			this._anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
		}
		
		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			string authTokenStorageKey = this._configuration["authTokenStorageKey"]!;
			string? token = await this._localStorageService.GetItemAsync<string>(authTokenStorageKey);

			//User is not logged in
			return string.IsNullOrWhiteSpace(token) ? 
				this._anonymous 
				: new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType")));
		}
		
		public void NotifyUserAuthentication(string token)
		{
			ClaimsPrincipal authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType"));

			Task<AuthenticationState> authState = Task.FromResult(new AuthenticationState(authenticatedUser));
			NotifyAuthenticationStateChanged(authState);
		}

		public void NotifyUserLogout()
		{
			Task<AuthenticationState> authState = Task.FromResult(this._anonymous);
			NotifyAuthenticationStateChanged(authState);
		}
	}
}