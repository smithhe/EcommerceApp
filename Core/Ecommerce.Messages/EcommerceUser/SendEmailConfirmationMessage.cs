namespace Ecommerce.Messages.EcommerceUser
{
    /// <summary>
    /// Message to send email confirmation after user registration
    /// </summary>
    public record SendEmailConfirmationMessage
    {
        /// <summary>
        /// Email address to send the email to
        /// </summary>
        public string SendTo { get; init; } = null!;
        
        /// <summary>
        /// Name of the user
        /// </summary>
        public string Name { get; init; } = null!;
        
        /// <summary>
        /// Link to confirm the email
        /// </summary>
        public string ConfirmationLink { get; init; } = null!;
    }
}