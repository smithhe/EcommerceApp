namespace Ecommerce.Shared.Security.Responses
{
    /// <summary>
    /// A response object for signing in a User
    /// </summary>
    public class AuthenticateResponse
    {
        /// <summary>
        /// The result of the sign in request
        /// </summary>
        public SignInResponseResult SignInResult { get; set; }
        
        /// <summary>
        /// The authentication token for the User
        /// </summary>
        public string? Token { get; set; }
        
        /// <summary>
        /// The Two Factor Authentication token for the User
        /// </summary>
        public string? TwoFactorToken { get; set; }
    }
}