using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.CartItem.Commands.DeleteUserCartItems;
using Ecommerce.Application.Features.Order.Commands.CreateOrder;
using Ecommerce.Application.Features.Order.Commands.UpdateOrder;
using Ecommerce.Application.Features.PayPal.Commands.CreatePayPalOrder;
using Ecommerce.Domain.Constants.Entities;
using Ecommerce.Domain.Constants.Identity;
using Ecommerce.FastEndpoints.Contracts;
using Ecommerce.Shared.Enums;
using Ecommerce.Shared.Exceptions;
using Ecommerce.Shared.Requests.Order;
using Ecommerce.Shared.Responses.CartItem;
using Ecommerce.Shared.Responses.Order;
using Ecommerce.Shared.Responses.PayPal;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ecommerce.FastEndpoints.Endpoints.Order
{
	/// <summary>
	/// A Fast Endpoint implementation that handles creating a new Order
	/// </summary>
	public class CreateOrderEndpoint : Endpoint<CreateOrderApiRequest, CreateOrderResponse>
	{
		private readonly ILogger<CreateOrderEndpoint> _logger;
		private readonly IMediator _mediator;
		private readonly ITokenService _tokenService;
		private readonly IConfiguration _configuration;

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateOrderEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		/// <param name="tokenService"> The <see cref="ITokenService"/> instance used for operations on Auth tokens passed in requests </param>
		/// <param name="configuration">The <see cref="IConfiguration"/> instance used for configuration settings.</param>
		public CreateOrderEndpoint(ILogger<CreateOrderEndpoint> logger, IMediator mediator, ITokenService tokenService, IConfiguration configuration)
		{
			this._logger = logger;
			this._mediator = mediator;
			this._tokenService = tokenService;
			this._configuration = configuration;
		}
		
		/// <summary>
		/// Configures the route and roles for the Endpoint
		/// </summary>
		public override void Configure()
		{
			this.Post("/api/order/create");
			this.Policies(PolicyNames._generalPolicy);
			this.Options(o => o.WithTags("Order"));
		}

		/// <summary>
		/// Handles the <see cref="CreateOrderApiRequest"/> and generates a <see cref="CreateOrderResponse"/> 
		/// </summary>
		/// <param name="req">The <see cref="CreateOrderApiRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(CreateOrderApiRequest req, CancellationToken ct)
		{
			//Log the request
			this._logger.LogInformation("Handling Create Order Request");
			
			//Check if token is valid
			string? token = this.HttpContext.Request.Headers.Authorization;
			if (await this._tokenService.ValidateTokenAsync(token) == false)
			{
				//Token is Invalid
				await this.SendUnauthorizedAsync(ct);
				return;
			}
			
			//Check if the request is valid
			if (ValidateRequest(req) == false)
			{
				//Invalid Request
				await this.SendAsync(new CreateOrderResponse { Success = false, Message = "Invalid Request" }, 400, ct);
				return;
			}


			//Create the Ecommerce Order
			CreateOrderResponse response;
			try
			{
				//Send the create command
				response = await this._mediator.Send(new CreateOrderCommand
				{
					CartItems = req.CartItems, 
					UserName = this._tokenService.GetUserNameFromToken(token),
					UserId = this._tokenService.GetUserIdFromToken(token)
				}, ct);
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempting to create Order");
				await this.SendAsync(new CreateOrderResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}
			
			//Verify the order was created
			if (response.Success == false)
			{
				//Order was not created send the response
				await this.SendOkAsync(response, ct);
				return;
			}
			
			//Create a payment with the specified payment source
			try
			{
				switch (req.PaymentSource)
				{
					case PaymentSource.Standard:
						await this.HandleStandardPaymentSource(response, ct);
						break;
					case PaymentSource.PayPal:
						await this.HandlePayPalPaymentSource(response, ct);
						break;
					default:
						throw new InvalidPaymentSourceException("Invalid Payment Source Provided");
				}
			}
			catch (InvalidPaymentSourceException)
			{
				this._logger.LogError($"Invalid Payment Source Provided");
				await this.SendAsync(new CreateOrderResponse { Success = false, Message = "Invalid Payment Source Provided" },
					400, ct);
				return;
			}
			catch (Exception e)
			{
				//Unexpected error
				this._logger.LogError(e, "Error when attempting to handle Payment Source");
				await this.SendAsync(new CreateOrderResponse { Success = false, Message = "Unexpected Error Occurred" },
					500, ct);
				return;
			}
			
			//Empty the cart if payment was successful
			if (response.Success)
			{
				DeleteUserCartItemsResponse deleteUserCartItemsResponse = await this._mediator.Send(new DeleteUserCartItemsCommand { UserId = this._tokenService.GetUserIdFromToken(token) ?? Guid.Empty }, ct);
				
				//Check if the cart empty was successful
				if (deleteUserCartItemsResponse.Success == false)
				{
					//Log the error and return false
					this._logger.LogError("Failed to empty the cart for the user");
					await this.SendAsync(new CreateOrderResponse { Success = false, Message = "Unexpected Error Occurred" },
						500, ct);
					return;
				}
			}

			//Send the response
			await this.SendOkAsync(response, ct);
		}

		/// <summary>
		/// Validates the <see cref="CreateOrderApiRequest"/> to ensure it is valid
		/// </summary>
		/// <param name="request">The <see cref="CreateOrderApiRequest"/> to validate</param>
		/// <returns>
		/// <c>true</c> if the request is valid; otherwise, <c>false</c>.
		/// </returns>
		private static bool ValidateRequest(CreateOrderApiRequest request)
		{
			//Check if we have any cart items
			if (request.CartItems == null || request.CartItems.Any() == false)
			{
				return false;
			}
			
			//Check if we have a valid payment source
			if (Enum.IsDefined(typeof(PaymentSource), request.PaymentSource) == false)
			{
				return false;
			}

			//Valid Request
			return true;
		}

		/// <summary>
		/// Handles skipping an official payment source and just updates the order status to Processing
		/// </summary>
		/// <param name="response">The <see cref="CreateOrderResponse"/> from creating the internal system order</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		private async Task HandleStandardPaymentSource(CreateOrderResponse response, CancellationToken ct)
		{
			//Get the UI Url from the configuration
			string? uiUrl = this._configuration["UIUrl"];
			
			//Update the order status to Pending
			response.Order!.Status = OrderStatus.Pending;
			
			//Update the order in the database
			UpdateOrderResponse updateOrderResponse = await this._mediator.Send(new UpdateOrderCommand
			{
				OrderToUpdate = response.Order!,
				UserName = "System"
			}, ct);
			
			//Check if the update was successful
			if (updateOrderResponse.Success == false)
			{
				//Order was not updated send the response
				response.Success = false;
				response.Message = OrderConstants._createErrorMessage;
				response.Order = null;
			}
			else
			{
				response.RedirectUrl = $"{uiUrl}/checkout/success/{response.Order!.Id}";
			}
		}
		
		/// <summary>
		/// Handles using PayPal as the payment source for the order
		/// </summary>
		/// <param name="response">The <see cref="CreateOrderResponse"/> from creating the internal system order</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		private async Task HandlePayPalPaymentSource(CreateOrderResponse response, CancellationToken ct)
		{
			//Create the PayPal Order
			CreatePayPalOrderResponse payPalResponse = await this._mediator.Send(new CreatePayPalOrderCommand
				{
					Order = response.Order!,
				}, ct);
			
			if (payPalResponse.Success)
			{
				//Add the RedirectUrl to the response
				response.RedirectUrl = payPalResponse.RedirectUrl;
			}
			else
			{
				response.Success = false;
				response.Message = payPalResponse.Message;
				response.Order = null;
				this._logger.LogWarning($"Failed to create PayPal Order{Environment.NewLine}{payPalResponse.Message}");
			}
		}
	}
}