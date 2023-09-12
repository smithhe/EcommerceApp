using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Category;
using MediatR;

namespace Ecommerce.Application.Features.Category.Commands.UpdateCategory
{
	/// <summary>
	/// A <see cref="Mediator"/> request for updating an existing <see cref="Category"/>
	/// </summary>
	public class UpdateCategoryCommand : IRequest<UpdateCategoryResponse>
	{
		/// <summary>
		/// The Category to update with
		/// </summary>
		public CategoryDto? CategoryToUpdate { get; set; }
	}
}