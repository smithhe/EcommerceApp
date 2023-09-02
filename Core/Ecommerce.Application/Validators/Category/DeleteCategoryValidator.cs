using Ecommerce.Application.Features.Category.Commands.DeleteCategory;
using FluentValidation;

namespace Ecommerce.Application.Validators.Category
{
	public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
	{
		public DeleteCategoryValidator()
		{
			RuleFor(c => c.CategoryToDelete!.Id)
				.GreaterThan(0).WithMessage("Must provide a Id greater than 0");
		}
	}
}