using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Responses.CartItem;
using MediatR;
using System;

namespace Ecommerce.Application.Features.CartItem.Commands.DeleteUserCartItems
{
	/// <summary>
	/// A <see cref="Mediator"/> request for deleting all of a User's <see cref="CartItem"/> entities
	/// </summary>
	public class DeleteUserCartItemsCommand : IRequest<DeleteUserCartItemsResponse>
	{
		/// <summary>
		/// The unique identifier of the User to delete all CartItems for
		/// </summary>
		public Guid UserId { get; set; }
	}
}