using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Product;
using MediatR;

namespace Ecommerce.Application.Features.Product.Commands.UpdateProduct
{
	/// <summary>
	/// A <see cref="Mediator"/> request for updating an existing <see cref="Product"/>
	/// </summary>
	public class UpdateProductCommand : IRequest<UpdateProductResponse>
	{
		/// <summary>
		/// The Product to update with
		/// </summary>
		public ProductDto? ProductToUpdate { get; set; }
	}
}