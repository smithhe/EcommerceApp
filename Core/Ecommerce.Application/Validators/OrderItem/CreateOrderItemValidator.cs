using Ecommerce.Application.Features.OrderItem.Commands;
using Ecommerce.Persistence.Contracts;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.Application.Validators.OrderItem
{
	public class CreateOrderItemValidator : AbstractValidator<CreateOrderItemCommand>
	{
		private readonly IProductAsyncRepository _productAsyncRepository;
		private readonly IOrderAsyncRepository _orderAsyncRepository;

		public CreateOrderItemValidator(IProductAsyncRepository productAsyncRepository, IOrderAsyncRepository orderAsyncRepository)
		{
			this._productAsyncRepository = productAsyncRepository;
			this._orderAsyncRepository = orderAsyncRepository;

			RuleFor(c => c.OrderItemToCreate.Quantity)
				.GreaterThan(0).WithMessage("Quantity must be greater than 0");

			RuleFor(c => c)
				.MustAsync(ProductExists).WithMessage("Product must exist");
			
			RuleFor(c => c)
				.MustAsync(OrderExists).WithMessage("Order must exist");
		}

		private async Task<bool> ProductExists(CreateOrderItemCommand command, CancellationToken cancellationToken)
		{
			return (await this._productAsyncRepository.GetByIdAsync(command.OrderItemToCreate.ProductId)) == null;
		}
		
		private async Task<bool> OrderExists(CreateOrderItemCommand command, CancellationToken cancellationToken)
		{
			return (await this._orderAsyncRepository.GetByIdAsync(command.OrderItemToCreate.OrderId)) == null;
		}
	}
}