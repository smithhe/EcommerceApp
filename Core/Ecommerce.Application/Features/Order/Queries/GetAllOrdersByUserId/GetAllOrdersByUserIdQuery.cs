using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Responses.Order;
using MediatR;
using System;

namespace Ecommerce.Application.Features.Order.Queries.GetAllOrdersByUserId
{
	/// <summary>
	/// A <see cref="Mediator"/> request for retrieving all <see cref="Order"/> entities for a User
	/// </summary>
	public class GetAllOrdersByUserIdQuery : IRequest<GetAllOrdersByUserIdResponse>
	{
		/// <summary>
		/// Id of the <see cref="EcommerceUser"/> to find all <see cref="Order"/> entities for
		/// </summary>
		public Guid UserId { get; set; }
	}
}