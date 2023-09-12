using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.CartItem;
using MediatR;

namespace Ecommerce.Application.Features.CartItem.Commands.UpdateCartItem
{
	/// <summary>
	/// A <see cref="Mediator"/> request for updating an existing <see cref="CartItem"/>
	/// </summary>
	public class UpdateCartItemCommand : IRequest<UpdateCartItemResponse>
	{
		/// <summary>
		/// The CartItem to update with
		/// </summary>
		public CartItemDto? CartItemToUpdate { get; set; }
		
		/// <summary>
		/// The User requesting to update the CartItem
		/// </summary>
		public string? UserName { get; set; }
	}
}