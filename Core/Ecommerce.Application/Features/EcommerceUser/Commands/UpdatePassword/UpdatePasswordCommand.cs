using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Security;
using MediatR;

namespace Ecommerce.Application.Features.EcommerceUser.Commands.UpdatePassword
{
    /// <summary>
    /// A <see cref="Mediator"/> request for updating an existing <see cref="EcommerceUser"/>'s password
    /// </summary>
    public class UpdatePasswordCommand : IRequest<UpdatePasswordResponse>
    {
        /// <summary>
        /// The UserName of the User to update the password for
        /// </summary>
        public string? UserName { get; set; }
    
        /// <summary>
        /// The current password of the User
        /// </summary>
        public string? CurrentPassword { get; set; }
    
        /// <summary>
        /// The new password to use
        /// </summary>
        public string? NewPassword { get; set; }
    }
}