using System.Threading.Tasks;
using Ecommerce.Mail.Contracts;
using Ecommerce.Mail.Models.Enums;
using Ecommerce.Mail.Models.TemplateModels;
using Ecommerce.Messages.Order;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Worker.Consumers.Order
{
    /// <summary>
    /// Consumer to send an order confirmation
    /// </summary>
    public class SendOrderConfirmationConsumer : IConsumer<SendOrderConfirmationMessage>
    {
        private readonly ILogger<SendOrderConfirmationConsumer> _logger;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendOrderConfirmationConsumer"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="emailService">The <see cref="IEmailService"/> instance used for sending an email</param>
        public SendOrderConfirmationConsumer(ILogger<SendOrderConfirmationConsumer> logger, IEmailService emailService)
        {
            this._logger = logger;
            this._emailService = emailService;
        }
        
        
        public async Task Consume(ConsumeContext<SendOrderConfirmationMessage> context)
        {
            //Log the message
            this._logger.LogInformation("Handling SendOrderConfirmationMessage");
            
            //Create the model for the email
            OrderReceiptModel model = new OrderReceiptModel
            {
                Name = context.Message.Name,
                Total = context.Message.Total,
                OrderNumber = context.Message.OrderNumber,
                OrderItems = context.Message.OrderItems
            };

            try
            {
                //Send the email
                await this._emailService.SendEmailAsync(context.Message.SendTo, "Order Confirmation", EmailTemplate.OrderReceipt, model);
            }
            catch (System.Exception e)
            {
                //Log the error
                this._logger.LogError(e, "Error sending order confirmation");
            }
        }
    }
}