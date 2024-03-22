using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Domain.Infrastructure;
using Ecommerce.Persistence.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.Features.PayPal.Commands.DeletePayPalReturnKey
{
    /// <summary>
    /// A <see cref="Mediator"/> request for deleting a PayPal return key
    /// </summary>
    public class DeletePayPalReturnKeyCommandHandler : IRequestHandler<DeletePayPalReturnKeyCommand, bool>
    {
        private readonly ILogger<DeletePayPalReturnKeyCommandHandler> _logger;
        private readonly IOrderKeyRepository _orderKeyRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeletePayPalReturnKeyCommandHandler"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
        /// <param name="orderKeyRepository">The <see cref="IOrderKeyRepository"/> instance used for data access for <see cref="OrderKey"/> entities.</param>
        public DeletePayPalReturnKeyCommandHandler(ILogger<DeletePayPalReturnKeyCommandHandler> logger, IOrderKeyRepository orderKeyRepository)
        {
            this._logger = logger;
            this._orderKeyRepository = orderKeyRepository;
        }
        
        /// <summary>
        /// Handles the <see cref="DeletePayPalReturnKeyCommand"/> request
        /// </summary>
        /// <param name="command">The <see cref="DeletePayPalReturnKeyCommand"/> request to be handled.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
        /// <returns>
        /// Returns <c>true</c> if the OrderKey was successfully deleted
        /// Returns <c>false</c> if the OrderKey was not deleted
        /// </returns>
        public async Task<bool> Handle(DeletePayPalReturnKeyCommand command, CancellationToken cancellationToken)
        {
            //Log the request to delete a PayPal return key
            this._logger.LogInformation("Handling request to delete a PayPal return key");

            //Check if the return key is null or empty
            if (string.IsNullOrEmpty(command.ReturnKey))
            {
                //Log the error and return false
                this._logger.LogWarning("The return key is null or empty");
                return false;
            }
            
            //Get the OrderKey from the database
            OrderKey? orderKey = await this._orderKeyRepository.GetByReturnKeyAsync(command.ReturnKey);
            
            //Check if the OrderKey was found
            if (orderKey == null)
            {
                //Log the error and return false
                this._logger.LogWarning("Failed to find the OrderKey with the return key");
                return false;
            }
            
            //Delete the OrderKey from the database
            bool success = await this._orderKeyRepository.DeleteAsync(orderKey);
            
            //Check if the delete was successful
            if (success == false)
            {
                //Log the error
                this._logger.LogError("Failed to delete the OrderKey from the database");
            }
            
            //Return the result of the delete
            return success;
        }
    }
}