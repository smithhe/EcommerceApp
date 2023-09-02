using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Category;
using MediatR;

namespace Ecommerce.Application.Features.Category.Commands.CreateCategory
{
	/// <summary>
	/// A <see cref="Mediator"/> request for creating a new Category
	/// </summary>
	public class CreateCategoryCommand : IRequest<CreateCategoryResponse>
	{
		/// <summary>
		/// The Category to be created
		/// </summary>
		public CategoryDto? CategoryToCreate { get; set; }
	}
}