using Ecommerce.Application.Features.Product.Commands.CreateProduct;
using Ecommerce.Persistence.Contracts;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.Application.Validators.Product
{
	public class CreateProductValidator : AbstractValidator<CreateProductCommand>
	{
		private readonly IProductAsyncRepository _productAsyncRepository;
		private readonly ICategoryAsyncRepository _categoryAsyncRepository;

		public CreateProductValidator(IProductAsyncRepository productAsyncRepository, ICategoryAsyncRepository categoryAsyncRepository)
		{
			this._productAsyncRepository = productAsyncRepository;
			this._categoryAsyncRepository = categoryAsyncRepository;
			
			RuleFor(c => c.ProductToCreate!.Name)
				.NotNull().WithMessage("Name cannot not be null")
				.NotEmpty().WithMessage("Name cannot not be empty")
				.MaximumLength(50).WithMessage("Name cannot exceed 50 characters");
			
			RuleFor(c => c)
				.MustAsync(NameIsUnique).WithMessage("Name must be unique");
			
			RuleFor(c => c.ProductToCreate!.Description)
				.NotNull().WithMessage("Description cannot not be null")
				.NotEmpty().WithMessage("Description cannot be empty")
				.MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

			RuleFor(c => c.ProductToCreate!.AverageRating)
				.GreaterThanOrEqualTo(0).WithMessage("Average Rating cannot be less than 0");
			
			RuleFor(c => c.ProductToCreate!.QuantityAvailable)
				.GreaterThanOrEqualTo(0).WithMessage("Quantity Available cannot be less than 0");
			
			RuleFor(c => c.ProductToCreate!.AverageRating)
				.GreaterThanOrEqualTo(0).WithMessage("Average Rating cannot be less than 0");
			
			RuleFor(c => c)
				.MustAsync(CategoryExists).WithMessage("Average Rating cannot be less than 0");
		}
		
		private async Task<bool> NameIsUnique(CreateProductCommand createProductCommand, CancellationToken cancellationToken)
		{
			return await this._productAsyncRepository.IsNameUnique(createProductCommand.ProductToCreate!.Name);
		}
		
		private async Task<bool> CategoryExists(CreateProductCommand createProductCommand, CancellationToken cancellationToken)
		{
			return (await this._categoryAsyncRepository.GetByIdAsync(createProductCommand.ProductToCreate!.Category.Id)) == null;
		}
	}
}