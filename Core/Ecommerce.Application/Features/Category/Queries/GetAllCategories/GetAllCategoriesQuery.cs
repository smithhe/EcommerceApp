using Ecommerce.Shared.Responses.Category;
using MediatR;

namespace Ecommerce.Application.Features.Category.Queries.GetAllCategories
{
	/// <summary>
	/// A <see cref="Mediator"/> request for retrieving all existing <see cref="Category"/> entities
	/// </summary>
	public class GetAllCategoriesQuery : IRequest<GetAllCategoriesResponse>;
}