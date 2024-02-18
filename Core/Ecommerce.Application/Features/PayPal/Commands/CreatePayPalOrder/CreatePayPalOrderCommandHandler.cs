using System.Threading;
using System.Threading.Tasks;
using Ecommerce.PayPal.Contracts;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Responses.PayPal;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Features.PayPal.Commands.CreatePayPalOrder
{
    public class CreatePayPalOrderCommandHandler : IRequestHandler<CreatePayPalOrderCommand, CreatePayPalOrderResponse>
    {
        private readonly ILogger<CreatePayPalOrderCommandHandler> _logger;
        private readonly IProductAsyncRepository _productAsyncRepository;
        private readonly IPaypalClientService _paypalClientService;

        public CreatePayPalOrderCommandHandler(ILogger<CreatePayPalOrderCommandHandler> logger, IProductAsyncRepository productAsyncRepository, 
            IPaypalClientService paypalClientService)
        {
            this._logger = logger;
            this._productAsyncRepository = productAsyncRepository;
            this._paypalClientService = paypalClientService;
        }
        
        public Task<CreatePayPalOrderResponse> Handle(CreatePayPalOrderCommand command, CancellationToken cancellationToken)
        {
            
            throw new System.NotImplementedException();
        }
    }
}