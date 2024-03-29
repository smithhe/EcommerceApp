namespace Ecommerce.Shared.Security.Responses
{
    /// <summary>
    /// A response object for confirming a User's email address
    /// </summary>
    public class ConfirmEmailResponse
    {
        /// <summary>
        /// Indicates whether the request was successful or not
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// The message returned from the request
        /// </summary>
        public string? Message { get; set; }
    }
}