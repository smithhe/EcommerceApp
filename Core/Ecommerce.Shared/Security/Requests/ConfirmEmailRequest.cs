
namespace Ecommerce.Shared.Security.Requests
{
    /// <summary>
    /// A API request for confirming a User's email address
    /// </summary>
    public class ConfirmEmailRequest
    {
        /// <summary>
        /// The unique identifier of the User to confirm the email for
        /// </summary>
        public string? UserId { get; set; }
        
        /// <summary>
        /// The token in the link sent to the User's email
        /// </summary>
        public string? EmailToken { get; set; }
    }
}