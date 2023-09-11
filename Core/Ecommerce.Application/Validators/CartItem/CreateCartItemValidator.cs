using Ecommerce.Application.Features.CartItem.Commands.CreateCartItem;
using Ecommerce.Persistence.Contracts;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.Application.Validators.CartItem
{
	public class CreateCartItemValidator : AbstractValidator<CreateCartItemCommand>
	{
		private readonly ICartItemRepository _cartItemRepository;
		private readonly IProductAsyncRepository _productAsyncRepository;

		public CreateCartItemValidator(ICartItemRepository cartItemRepository, IProductAsyncRepository productAsyncRepository)
		{
			this._cartItemRepository = cartItemRepository;
			this._productAsyncRepository = productAsyncRepository;

			RuleFor(c => c)
				.MustAsync(CartItemDoesNotExist).WithMessage("Cart Item Already Exists")
				.MustAsync(ProductExists).WithMessage("Product must exist");

			RuleFor(c => c.CartItemToCreate!.Quantity)
				.GreaterThan(0).WithMessage("Quantity Must Be Greater Than 0");
		}
		
		private async Task<bool> CartItemDoesNotExist(CreateCartItemCommand createCartItemCommand, CancellationToken cancellationToken)
		{
			return (await this._cartItemRepository.CartItemExistsForUser(createCartItemCommand.CartItemToCreate!.UserId,
				createCartItemCommand.CartItemToCreate.ProductId)) == false;
		}
		
		private async Task<bool> ProductExists(CreateCartItemCommand command, CancellationToken cancellationToken)
		{
			return (await this._productAsyncRepository.GetByIdAsync(command.CartItemToCreate!.ProductId)) != null;
		}
	}
}