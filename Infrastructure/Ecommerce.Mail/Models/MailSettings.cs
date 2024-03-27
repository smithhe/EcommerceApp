namespace Ecommerce.Mail.Models
{
    /// <summary>
    /// Model for mail settings
    /// </summary>
    public class MailSettings
    {
        /// <summary>
        /// The host address of the mail server
        /// </summary>
        public string? Host { get; set; }
        
        /// <summary>
        /// The port to use on the mail server
        /// </summary>
        public int Port { get; set; }
        
        /// <summary>
        /// The email address to send mail from
        /// </summary>
        public string? UserName { get; set; }
        
        /// <summary>
        /// The password to use to authenticate with the mail server
        /// </summary>
        public string? Password { get; set; }
    }
}