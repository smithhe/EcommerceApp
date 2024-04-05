using System.Threading.Tasks;
using Ecommerce.Mail.Models.Enums;

namespace Ecommerce.Mail.Contracts
{
    /// <summary>
    /// Contract for sending emails
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email asynchronously
        /// </summary>
        /// <param name="sendTo">The email address to send to</param>
        /// <param name="subject">The subject of the email</param>
        /// <param name="templateToUse">The email template to use</param>
        /// <param name="templateModel">The model with the information to fill into the template</param>
        Task SendEmailAsync(string sendTo, string subject, EmailTemplate templateToUse, ITemplateModel templateModel);
    }
}