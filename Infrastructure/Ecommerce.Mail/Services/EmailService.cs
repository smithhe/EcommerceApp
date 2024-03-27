using System;
using System.Threading.Tasks;
using Ecommerce.Mail.Contracts;
using Ecommerce.Mail.Models;
using Ecommerce.Mail.Models.Enums;
using FluentEmail.Core;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Mail.Services
{
    /// <summary>
    /// Service to send emails
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IFluentEmail _fluentEmail;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="fluentEmail">The <see cref="IFluentEmail"/> instance used for sending a single email</param>
        public EmailService(ILogger<EmailService> logger, IFluentEmail fluentEmail)
        {
            this._logger = logger;
            this._fluentEmail = fluentEmail;
        }
        
        /// <summary>
        /// Sends an email asynchronously
        /// </summary>
        /// <param name="sendTo">The email address to send to</param>
        /// <param name="subject">The subject of the email</param>
        /// <param name="templateToUse">The email template to use</param>
        /// <param name="templateModel">The model with the information to fill into the template</param>
        public async Task SendEmailAsync(string sendTo, string subject, EmailTemplate templateToUse, ITemplateModel templateModel)
        {
            IFluentEmail? email = this._fluentEmail
                .To(sendTo)
                .Subject(subject);
            
            this.SetTemplate(templateToUse, templateModel, email);
            
            await email.SendAsync();
        }
        
        /// <summary>
        /// Load the template and set it in the email
        /// </summary>
        /// <param name="templateToUse">The email template to use</param>
        /// <param name="templateModel">The model with the information to fill into the template</param>
        /// <param name="email">The email to load the template into</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the template is not found in <see cref="EmailTemplate"/></exception>
        private void SetTemplate(EmailTemplate templateToUse, ITemplateModel templateModel, IFluentEmail email)
        {
            switch (templateToUse)
            {
                case EmailTemplate.EmailConfirmation:
                    email.UsingTemplateFromEmbedded("Ecommerce.Mail.Templates.EmailConfirmation.cshtml", templateModel, this.GetType().Assembly, true);
                    break;
                case EmailTemplate.OrderReceipt:
                    email.UsingTemplateFromEmbedded("Ecommerce.Mail.Templates.OrderReceipt.cshtml", templateModel, this.GetType().Assembly, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(templateToUse), templateToUse, "Template not found");
            }
        }
    }
}