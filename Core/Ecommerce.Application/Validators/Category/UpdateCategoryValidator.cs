using Ecommerce.Application.Features.Category.Commands.UpdateCategory;
using Ecommerce.Persistence.Contracts;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.Application.Validators.Category
{
	public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
	{
		private readonly ICategoryAsyncRepository _categoryAsyncRepository;

		public UpdateCategoryValidator(ICategoryAsyncRepository categoryAsyncRepository)
		{
			this._categoryAsyncRepository = categoryAsyncRepository;
			
			RuleFor(c => c.CategoryToUpdate!.Name)
				.NotNull().WithMessage("Name cannot not be null")
				.NotEmpty().WithMessage("Name cannot not be empty")
				.MaximumLength(50).WithMessage("Name cannot exceed 50 characters");

			RuleFor(c => c)
				.MustAsync(NameIsUnique).WithMessage("Name must be unique");

			RuleFor(c => c.CategoryToUpdate!.Summary)
				.NotNull().WithMessage("Summary cannot not be null")
				.NotEmpty().WithMessage("Summary cannot be empty")
				.MaximumLength(50).WithMessage("Summary cannot exceed 50 characters");
		}
		
		private async Task<bool> NameIsUnique(UpdateCategoryCommand updateCategoryCommand, CancellationToken cancellationToken)
		{
			return await this._categoryAsyncRepository.IsNameUnique(updateCategoryCommand.CategoryToUpdate!.Name);
		}
	}
}