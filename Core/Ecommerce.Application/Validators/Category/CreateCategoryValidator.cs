using Ecommerce.Application.Features.Category.Commands.CreateCategory;
using FluentValidation;

namespace Ecommerce.Application.Validators.Category
{
	public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
	{
		public CreateCategoryValidator()
		{
			RuleFor(c => c.CategoryToCreate!.Name)
				.NotNull().WithMessage("Name cannot not be null")
				.NotEmpty().WithMessage("Name cannot not be empty")
				.MaximumLength(50).WithMessage("Name cannot exceed 50 characters");

			RuleFor(c => c.CategoryToCreate!.Description)
				.NotNull().WithMessage("Description cannot not be null")
				.NotEmpty().WithMessage("Description cannot be empty")
				.MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
		}	
	}
}