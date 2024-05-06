using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Ecommerce.UI.Extensions;
using Microsoft.JSInterop;

namespace Ecommerce.UI.Security
{
	public class AuthStateProvider : AuthenticationStateProvider
	{
		private readonly ILocalStorageService _localStorageService;
		private readonly IConfiguration _configuration;
		private readonly IJSRuntime _jsRuntime;
		private readonly AuthenticationState _anonymous;

		public AuthStateProvider(ILocalStorageService localStorageService, IConfiguration configuration, IJSRuntime jsRuntime)
		{
			this._localStorageService = localStorageService;
			this._configuration = configuration;
			this._jsRuntime = jsRuntime;
			this._anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
		}
		
		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			string authTokenStorageKey = this._configuration["authTokenStorageKey"]!;
			string? token = await this._localStorageService.GetItemAsync<string>(authTokenStorageKey);

			if (string.IsNullOrWhiteSpace(token))
			{
				return this._anonymous;
			}

			if (IsTokenExpired(token))
			{
				return this._anonymous;
			}
			
			return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType")));
		}

		private bool IsTokenExpired(string token)
		{
			// Decode the JWT token
			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
			JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

			if (jwtToken == null)
			{
				// Invalid token format
				return true;
			}

			if (jwtToken.ValidTo <= DateTime.UtcNow)
			{
				// Token has expired
				return true;
			}

			// Token is still valid
			return false; 
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