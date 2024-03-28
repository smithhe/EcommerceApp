using System;
using Ecommerce.Shared.Security;
using FastEndpoints;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.EcommerceUser.Commands.RegisterEcommerceUser;
using MediatR;

namespace Ecommerce.FastEndpoints.Security
{
	/// <summary>
	/// A Fast Endpoint implementation that handles registering a new User
	/// </summary>
	public class RegisterEndpoint : Endpoint<CreateUserRequest, CreateUserResponse>
	{
		private readonly ILogger<RegisterEndpoint> _logger;
		private readonly IMediator _mediator;

		/// <summary>
		/// Initializes a new instance of the <see cref="RegisterEndpoint"/> class.
		/// </summary>
		/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
		/// <param name="mediator">The <see cref="IMediator"/> instance used for sending Mediator requests.</param>
		public RegisterEndpoint(ILogger<RegisterEndpoint> logger, IMediator mediator)
		{
			this._logger = logger;
			this._mediator = mediator;
		}
		
		/// <summary>
		/// Configures the route for the Endpoint
		/// </summary>
		public override void Configure()
		{
			this.Post("/api/register");
			this.AllowAnonymous();
		}

		/// <summary>
		/// Handles the <see cref="CreateUserRequest"/> and generates a <see cref="CreateUserResponse"/> if registration was successful
		/// </summary>
		/// <param name="req">The <see cref="CreateUserRequest"/> object sent in the HTTP request</param>
		/// <param name="ct">The <see cref="CancellationToken"/> that can be used to request cancellation of the operation.</param>
		public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
		{
			//Log the request
			this._logger.LogInformation("Handling request to register a new User");
			
			//Attempt to register the user
			CreateUserResponse response;
			try
			{
				//Send the request to the Mediator
				response = await this._mediator.Send(new RegisterEcommerceUserCommand
				{
					CreateUserRequest = req,
					LinkUrl = $"https://{this.HttpContext.Request.Host}/api/confirm"
				}, ct);
			}
			catch (Exception e)
			{
				//Log the exception
				this._logger.LogError(e, "An error occurred while registering a new User");
				await this.SendAsync(new CreateUserResponse { Success = false, Errors = new string[] { "Unexpected Error Occurred" } },
					500, ct);
				return;
			}
			
			//Remove the confirmation link from the response
			response.ConfirmationLink = null;

			//Failed registration
			if (response.Success == false)
			{
				await this.SendAsync(response, 400, cancellation: ct);
				return;
			}
			
			//Return success
			await this.SendAsync(response, 201, cancellation: ct);
		}
	}
}