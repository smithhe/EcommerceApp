using System;
using Ecommerce.Shared.Security.Responses;
using MediatR;

namespace Ecommerce.Application.Features.EcommerceUser.Commands.ConfirmEmail
{
    /// <summary>
    /// A <see cref="Mediator"/> command for confirming a User's email address
    /// </summary>
    public class ConfirmEmailCommand : IRequest<ConfirmEmailResponse>
    {
        /// <summary>
        /// The unique identifier of the User to confirm
        /// </summary>
        public string UserId { get; set; } = null!;
        
        /// <summary>
        /// The token used to confirm the User's email address
        /// </summary>
        public string Token { get; set; } = null!;
    }
}