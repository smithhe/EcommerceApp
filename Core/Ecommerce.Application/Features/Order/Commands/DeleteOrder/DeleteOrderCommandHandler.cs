using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Ecommerce.Domain.Constants.Entities;
using Ecommerce.Persistence.Contracts;
using Ecommerce.Shared.Responses.Order;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Features.Order.Commands.DeleteOrder
{
    /// <summary>
    /// A <see cref="Mediator"/> request handler for <see cref="DeleteOrderCommand"/>
    /// </summary>
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, DeleteOrderResponse>
    {
        private readonly ILogger<DeleteOrderCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IOrderAsyncRepository _orderAsyncRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteOrderCommandHandler"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="mapper">The <see cref="IMapper"/> instance used for mapping objects.</param>
        /// <param name="orderAsyncRepository">The <see cref="IOrderAsyncRepository"/> instance used for data access for <see cref="Order"/> entities.</param>
        public DeleteOrderCommandHandler(ILogger<DeleteOrderCommandHandler> logger, IMapper mapper, IOrderAsyncRepository orderAsyncRepository)
        {
            this._logger = logger;
            this._mapper = mapper;
            this._orderAsyncRepository = orderAsyncRepository;
        }
        
        /// <summary>
        /// Handles the <see cref="DeleteOrderCommand"/> request
        /// </summary>
        /// <param name="command">The <see cref="DeleteOrderCommand"/> request to be handled.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
        /// <returns>
        /// A <see cref="DeleteOrderResponse"/> with Success being <c>true</c> if the <see cref="Order"/> was deleted;
        /// Success will be <c>false</c> if no <see cref="Order"/> is found or validation of the command fails.
        /// Message will contain the message to display to the user.
        /// </returns>
        public async Task<DeleteOrderResponse> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
        {
            //Log the request
            this._logger.LogInformation("Handling request to delete an order");

            //Create the response object
            DeleteOrderResponse response = new DeleteOrderResponse { Success = true, Message = OrderConstants._deleteSuccessMessage };
            
            //Check if the order dto is null
            if (command.Order == null)
            {
                //Log the error and return a failed response
                this._logger.LogWarning("Order was null in command, returning failed response");
                response.Success = false;
                response.Message = OrderConstants._deleteErrorMessage;
                return response;
            }
            
            //Delete the order from the database
            bool deleteResult = await this._orderAsyncRepository.DeleteAsync(this._mapper.Map<Domain.Entities.Order>(command.Order));
            
            //Check if the delete was successful
            if (deleteResult == false)
            {
                //Log the error and return a failed response
                this._logger.LogError("Failed to delete the order from the database");
                response.Success = false;
                response.Message = OrderConstants._deleteErrorMessage;
                return response;
            }
            
            //Return the successful response
            return response;
        }
    }
}