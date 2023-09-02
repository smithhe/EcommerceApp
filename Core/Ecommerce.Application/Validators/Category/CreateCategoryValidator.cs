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

			RuleFor(c => c.CategoryToCreate!.Description)
				.NotNull().WithMessage("Description cannot not be null")
				.NotEmpty().WithMessage("Description cannot be empty")
				.MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
		}
		
		private async Task<bool> NameIsUnique(CreateCategoryCommand createEmployeeCommand, CancellationToken cancellationtoken)
		{
			return await this._categoryAsyncRepository.IsNameUnique(createEmployeeCommand.CategoryToCreate!.Name);
		}
	}
}