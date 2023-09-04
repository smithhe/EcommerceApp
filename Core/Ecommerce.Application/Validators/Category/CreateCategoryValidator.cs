using Ecommerce.Application.Features.Category.Commands.CreateCategory;
using Ecommerce.Persistence.Contracts;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.Application.Validators.Category
{
	public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
	{
		private readonly ICategoryAsyncRepository _categoryAsyncRepository;
		
		public CreateCategoryValidator(ICategoryAsyncRepository categoryAsyncRepository)
		{
			this._categoryAsyncRepository = categoryAsyncRepository;
			
			RuleFor(c => c.CategoryToCreate!.Name)
				.NotNull().WithMessage("Name cannot not be null")
				.NotEmpty().WithMessage("Name cannot not be empty")
				.MaximumLength(50).WithMessage("Name cannot exceed 50 characters");

			RuleFor(c => c)
				.MustAsync(NameIsUnique).WithMessage("Name must be unique");

			RuleFor(c => c.CategoryToCreate!.Summary)
				.NotNull().WithMessage("Summary cannot not be null")
				.NotEmpty().WithMessage("Summary cannot be empty")
				.MaximumLength(50).WithMessage("Summary cannot exceed 50 characters");
		}
		
		private async Task<bool> NameIsUnique(CreateCategoryCommand createCategoryCommand, CancellationToken cancellationToken)
		{
			return await this._categoryAsyncRepository.IsNameUnique(createCategoryCommand.CategoryToCreate!.Name);
		}
	}
}