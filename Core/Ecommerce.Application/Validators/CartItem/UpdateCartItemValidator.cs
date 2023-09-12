using Ecommerce.Application.Features.CartItem.Commands.UpdateCartItem;
using Ecommerce.Persistence.Contracts;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.Application.Validators.CartItem
{
	public class UpdateCartItemValidator : AbstractValidator<UpdateCartItemCommand>
	{
		private readonly ICartItemRepository _cartItemRepository;
		private readonly IProductAsyncRepository _productAsyncRepository;

		public UpdateCartItemValidator(ICartItemRepository cartItemRepository, IProductAsyncRepository productAsyncRepository)
		{
			this._cartItemRepository = cartItemRepository;
			this._productAsyncRepository = productAsyncRepository;
			
			RuleFor(c => c)
				.MustAsync(CartItemExists).WithMessage("Cart Item must exist")
				.MustAsync(ProductExists).WithMessage("Product must exist");

			RuleFor(c => c.CartItemToUpdate!.Quantity)
				.GreaterThan(0).WithMessage("Quantity Must Be Greater Than 0");
		}
		
		private async Task<bool> CartItemExists(UpdateCartItemCommand updateCartItemCommand, CancellationToken cancellationToken)
		{
			return await this._cartItemRepository.CartItemExistsForUser(updateCartItemCommand.CartItemToUpdate!.UserId, updateCartItemCommand.CartItemToUpdate.ProductId);
		}
		
		private async Task<bool> ProductExists(UpdateCartItemCommand updateCartItemCommand, CancellationToken cancellationToken)
		{
			return (await this._productAsyncRepository.GetByIdAsync(updateCartItemCommand.CartItemToUpdate!.ProductId)) != null;
		}
	}
}