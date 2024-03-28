using System;
using System.Threading.Tasks;
using Ecommerce.Mail.Contracts;
using Ecommerce.Mail.Models.Enums;
using Ecommerce.Mail.Models.TemplateModels;
using Ecommerce.Messages.EcommerceUser;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Worker.Consumers.EcommerceUser
{
    /// <summary>
    /// Consumer to send an email confirmation
    /// </summary>
    public class SendEmailConfirmationConsumer : IConsumer<SendEmailConfirmationMessage>
    {
        private readonly ILogger<SendEmailConfirmationConsumer> _logger;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendEmailConfirmationConsumer"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="emailService">The <see cref="IEmailService"/> instance used for sending an email</param>
        public SendEmailConfirmationConsumer(ILogger<SendEmailConfirmationConsumer> logger, IEmailService emailService)
        {
            this._logger = logger;
            this._emailService = emailService;
        }
        
        /// <summary>
        /// Consumes the message to send an email confirmation
        /// </summary>
        /// <param name="context">The message to be handled</param>
        public async Task Consume(ConsumeContext<SendEmailConfirmationMessage> context)
        {
            //Log the message
            this._logger.LogInformation("Handling SendEmailConfirmationMessage");
            
            //Create the model for the email
            EmailConfirmationModel model = new EmailConfirmationModel
            {
                Name = context.Message.Name,
                CompanyName = context.Message.CompanyName,
                ConfirmationLink = context.Message.ConfirmationLink
            };

            try
            {
                //Send the email
                await this._emailService.SendEmailAsync(context.Message.SendTo, "Email Confirmation", EmailTemplate.EmailConfirmation, model);
            }
            catch (Exception e)
            {
                //Log the error
                this._logger.LogError(e, "Error sending email confirmation");
            }
        }
    }
}