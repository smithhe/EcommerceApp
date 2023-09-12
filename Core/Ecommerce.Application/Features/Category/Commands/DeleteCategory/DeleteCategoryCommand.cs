using Ecommerce.Domain.Entities;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.Category;
using MediatR;

namespace Ecommerce.Application.Features.Category.Commands.DeleteCategory
{
	/// <summary>
	/// A <see cref="Mediator"/> request for deleting a <see cref="Category"/>
	/// </summary>
	public class DeleteCategoryCommand : IRequest<DeleteCategoryResponse>
	{
		/// <summary>
		/// The Category to delete
		/// </summary>
		public CategoryDto? CategoryToDelete { get; set; }
	}
}