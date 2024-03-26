using System.Threading.Tasks;
using Ecommerce.Messages.EcommerceUser;
using MassTransit;

namespace Ecommerce.Worker.Consumers.EcommerceUser
{
    public class SendEmailConfirmationConsumer : IConsumer<SendEmailConfirmationMessage>
    {
        public SendEmailConfirmationConsumer()
        {
            
        }
        
        public Task Consume(ConsumeContext<SendEmailConfirmationMessage> context)
        {
            throw new System.NotImplementedException();
        }
    }
}