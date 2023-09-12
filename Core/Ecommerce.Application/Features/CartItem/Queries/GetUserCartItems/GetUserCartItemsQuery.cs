using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Responses.CartItem;
using MediatR;
using System;

namespace Ecommerce.Application.Features.CartItem.Queries.GetUserCartItems
{
	/// <summary>
	/// A <see cref="Mediator"/> request for retrieving all existing <see cref="CartItem"/> entities for an <see cref="EcommerceUser"/>
	/// </summary>
	public class GetUserCartItemsQuery : IRequest<GetUserCartItemsResponse>
	{
		/// <summary>
		/// Id of the <see cref="EcommerceUser"/> to find all <see cref="CartItem"/> entities for
		/// </summary>
		public Guid UserId { get; set; }
	}
}