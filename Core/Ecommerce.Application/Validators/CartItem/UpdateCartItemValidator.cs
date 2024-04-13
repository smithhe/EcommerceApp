using Ecommerce.Application.Features.CartItem.Commands.UpdateCartItem;
using Ecommerce.Persistence.Contracts;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Product.Queries.GetProductById;
using MediatR;

namespace Ecommerce.Application.Validators.CartItem
{
	public class UpdateCartItemValidator : AbstractValidator<UpdateCartItemCommand>
	{
		private readonly ICartItemRepository _cartItemRepository;
		private readonly IMediator _mediator;

		public UpdateCartItemValidator(ICartItemRepository cartItemRepository, IMediator mediator)
		{
			this._cartItemRepository = cartItemRepository;
			this._mediator = mediator;

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
			return (await this._mediator.Send(new GetProductByIdQuery { Id = updateCartItemCommand.CartItemToUpdate!.ProductId }, cancellationToken)).Product != null;
		}
	}
}