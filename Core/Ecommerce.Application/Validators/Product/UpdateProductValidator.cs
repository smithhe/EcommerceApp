using Ecommerce.Application.Features.Product.Commands.UpdateProduct;
using Ecommerce.Persistence.Contracts;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Category.Queries.GetCategoryById;
using MediatR;

namespace Ecommerce.Application.Validators.Product
{
	public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
	{
		private readonly IProductAsyncRepository _productAsyncRepository;
		private readonly IMediator _mediator;

		public UpdateProductValidator(IProductAsyncRepository productAsyncRepository, IMediator mediator)
		{
			this._productAsyncRepository = productAsyncRepository;
			this._mediator = mediator;

			RuleFor(c => c)
				.MustAsync(ProductExists).WithMessage("Product must exist to update");
			
			RuleFor(c => c.ProductToUpdate!.Name)
				.NotNull().WithMessage("Name cannot not be null")
				.NotEmpty().WithMessage("Name cannot not be empty")
				.MaximumLength(100).WithMessage("Name cannot exceed 100 characters");
			
			RuleFor(c => c)
				.MustAsync(NameIsUnique).WithMessage("Name must be unique");
			
			RuleFor(c => c.ProductToUpdate!.Description)
				.NotNull().WithMessage("Description cannot not be null")
				.NotEmpty().WithMessage("Description cannot be empty")
				.MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

			RuleFor(c => c.ProductToUpdate!.AverageRating)
				.GreaterThanOrEqualTo(0).WithMessage("Average Rating cannot be less than 0");
			
			RuleFor(c => c.ProductToUpdate!.QuantityAvailable)
				.GreaterThanOrEqualTo(0).WithMessage("Quantity Available cannot be less than 0");
			
			RuleFor(c => c)
				.MustAsync(CategoryExists).WithMessage("Category must exist");
		}

		private async Task<bool> ProductExists(UpdateProductCommand updateProductCommand, CancellationToken cancellationToken)
		{
			return (await this._productAsyncRepository.GetByIdAsync(updateProductCommand.ProductToUpdate!.Id)) != null;
		}
		
		private async Task<bool> NameIsUnique(UpdateProductCommand updateProductCommand, CancellationToken cancellationToken)
		{
			return await this._productAsyncRepository.IsNameUnique(updateProductCommand.ProductToUpdate!.Name);
		}
		
		private async Task<bool> CategoryExists(UpdateProductCommand updateProductCommand, CancellationToken cancellationToken)
		{
			return (await this._mediator.Send(new GetCategoryByIdQuery { Id = updateProductCommand.ProductToUpdate!.CategoryId }, cancellationToken)).Category != null;
		}
	}
}