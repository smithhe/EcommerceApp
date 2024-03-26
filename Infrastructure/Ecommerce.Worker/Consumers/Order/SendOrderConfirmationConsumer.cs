using System.Threading.Tasks;
using Ecommerce.Messages.Order;
using MassTransit;

namespace Ecommerce.Worker.Consumers.Order
{
    public class SendOrderConfirmationConsumer : IConsumer<SendOrderConfirmationMessage>
    {
        public SendOrderConfirmationConsumer()
        {
            
        }
        
        public Task Consume(ConsumeContext<SendOrderConfirmationMessage> context)
        {
            throw new System.NotImplementedException();
        }
    }
}