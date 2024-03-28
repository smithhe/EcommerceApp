using Ecommerce.Shared.Security;
using MediatR;

namespace Ecommerce.Application.Features.EcommerceUser.Commands.RegisterEcommerceUser
{
	/// <summary>
	/// A <see cref="Mediator"/> request for registering a new User
	/// </summary>
    public class RegisterEcommerceUserCommand : IRequest<CreateUserResponse>
    {
	    /// <summary>
	    /// The <see cref="CreateUserRequest"/> object that contains the User information to register
	    /// </summary>
	    public CreateUserRequest CreateUserRequest { get; set; } = null!;
	    
	    /// <summary>
	    /// The link to combine with the token to confirm the User's email address
	    /// </summary>
	    public string LinkUrl { get; set; } = null!;
    }
}