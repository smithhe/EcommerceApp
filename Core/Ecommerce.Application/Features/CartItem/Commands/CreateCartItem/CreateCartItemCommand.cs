using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.CartItem;
using MediatR;

namespace Ecommerce.Application.Features.CartItem.Commands.CreateCartItem
{
	/// <summary>
	/// A <see cref="Mediator"/> request for creating a new <see cref="CartItem"/>
	/// </summary>
	public class CreateCartItemCommand : IRequest<CreateCartItemResponse>
	{
		/// <summary>
		/// The CartItem to be created
		/// </summary>
		public CartItemDto? CartItemToCreate { get; set; }
		
		/// <summary>
		/// The User requesting to create the CartItem
		/// </summary>
		public string? UserName { get; set; }
	}
}