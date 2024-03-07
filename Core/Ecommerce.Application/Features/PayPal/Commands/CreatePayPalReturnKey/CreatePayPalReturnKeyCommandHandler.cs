using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Order.Queries.GetOrderById;
using Ecommerce.Domain.Infrastructure;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Responses.Order;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Features.PayPal.Commands.CreatePayPalReturnKey
{
    /// <summary>
    /// A <see cref="Mediator"/> request handler for <see cref="CreatePayPalReturnKeyCommand"/>
    /// </summary>
    public class CreatePayPalReturnKeyCommandHandler : IRequestHandler<CreatePayPalReturnKeyCommand, string?>
    {
        private readonly ILogger<CreatePayPalReturnKeyCommandHandler> _logger;
        private readonly IMediator _mediator;
        private readonly IOrderKeyRepository _orderKeyRepository;
        private readonly Random _random;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePayPalReturnKeyCommandHandler"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
        /// <param name="orderKeyRepository">The <see cref="IOrderKeyRepository"/> instance used for data access for <see cref="OrderKey"/> entities.</param>
        public CreatePayPalReturnKeyCommandHandler(ILogger<CreatePayPalReturnKeyCommandHandler> logger, IMediator mediator
            , IOrderKeyRepository orderKeyRepository)
        {
            this._logger = logger;
            this._mediator = mediator;
            this._orderKeyRepository = orderKeyRepository;
            this._random = new Random();
        }
        
        /// <summary>
        /// Handles the <see cref="CreatePayPalReturnKeyCommand"/> request
        /// </summary>
        /// <param name="command">The <see cref="CreatePayPalReturnKeyCommand"/> request to be handled.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
        /// <returns>
        /// A <see cref="string"/> with the return key for the PayPal order;
        /// <c>null</c> if the key could not be created
        /// </returns>
        public async Task<string?> Handle(CreatePayPalReturnKeyCommand command, CancellationToken cancellationToken)
        {
            //Log the request to create a new PayPal return key
            this._logger.LogInformation("Handling request to create a new PayPal return key");
            
            //Verify the order exists
            GetOrderByIdResponse orderResponse = await this._mediator.Send(new GetOrderByIdQuery { Id = command.OrderId }, cancellationToken);

            if (orderResponse.Order == null || orderResponse.Success == false)
            {
                this._logger.LogWarning($"Failed to find order with id {command.OrderId} when creating PayPal return key");
                return null;
            }
            
            //Create the PayPal return key
            string orderToken = this.GenerateRandomAlphanumericString(50);
            int newId = await this._orderKeyRepository.AddAsync(new OrderKey
            {
                OrderId = command.OrderId,
                OrderToken = orderToken,
                CreatedAt = DateTime.UtcNow
            });
            
            //Verify the key was created
            if (newId == -1)
            {
                this._logger.LogWarning($"Failed to create PayPal return key for order with id {command.OrderId}");
                return null;
            }
            
            //Return the key
            return orderToken;
        }

        /// <summary>
        /// Generates a random alphanumeric string of the given length
        /// </summary>
        /// <param name="length">The length of the random string</param>
        /// <returns>
        /// A random alphanumeric <see cref="string"/> of the given length
        /// </returns>
        private string GenerateRandomAlphanumericString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[this._random.Next(s.Length)]).ToArray());
        }
    }
}