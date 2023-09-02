using Ecommerce.Shared.Responses.Category;
using MediatR;

namespace Ecommerce.Application.Features.Category.Queries.GetCategoryById
{
	/// <summary>
	/// A <see cref="Mediator"/> request for retrieving an existing <see cref="Category"/> by its Id
	/// </summary>
	public class GetCategoryByIdQuery : IRequest<GetCategoryByIdResponse>
	{
		/// <summary>
		/// The unique identifier of the <see cref="Category"/> to retrieve
		/// </summary>
		public int Id { get; set; }
	}
}