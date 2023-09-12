using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Product;
using MediatR;

namespace Ecommerce.Application.Features.Product.Commands.CreateProduct
{
	/// <summary>
	/// A <see cref="Mediator"/> request for creating a new <see cref="Product"/>
	/// </summary>
	public class CreateProductCommand : IRequest<CreateProductResponse>
	{
		/// <summary>
		/// The Product to be created
		/// </summary>
		public ProductDto? ProductToCreate { get; set; }
	}
}