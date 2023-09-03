using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Product;
using MediatR;

namespace Ecommerce.Application.Features.Product.Commands.DeleteProduct
{
	/// <summary>
	/// A <see cref="Mediator"/> request for deleting a <see cref="Product"/>
	/// </summary>
	public class DeleteProductCommand : IRequest<DeleteProductResponse>
	{
		/// <summary>
		/// The <see cref="Product"/> to delete
		/// </summary>
		public ProductDto? ProductToDelete { get; set; }
	}
}