using Blazored.LocalStorage;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.UI.Security
{
	public class AuthHeaderHandler : DelegatingHandler
	{
		private readonly ILocalStorageService _localStorageService;
		private readonly string _authTokenStorageKey;

		public AuthHeaderHandler(ILocalStorageService localStorageService, IConfiguration configuration)
		{
			this._localStorageService = localStorageService;
			this._authTokenStorageKey = configuration["authTokenStorageKey"]!;
		}
		
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			string? token = await this._localStorageService.GetItemAsync<string>(this._authTokenStorageKey, cancellationToken);

			if (string.IsNullOrEmpty(token) == false)
			{
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			}
			
			return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
		}
	}
}