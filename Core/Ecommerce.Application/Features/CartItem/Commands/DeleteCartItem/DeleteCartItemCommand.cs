using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.CartItem;
using MediatR;

namespace Ecommerce.Application.Features.CartItem.Commands.DeleteCartItem
{
	/// <summary>
	/// A <see cref="Mediator"/> request for deleting a <see cref="CartItem"/>
	/// </summary>
	public class DeleteCartItemCommand : IRequest<DeleteCartItemResponse>
	{
		/// <summary>
		/// The CartItem to delete
		/// </summary>
		public CartItemDto? CartItemToDelete { get; set; }
	}
}