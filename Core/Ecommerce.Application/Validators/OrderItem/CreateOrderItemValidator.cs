using Ecommerce.Application.Features.OrderItem.Commands.CreateOrderItem;
using Ecommerce.Persistence.Contracts;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.Application.Validators.OrderItem
{
	public class CreateOrderItemValidator : AbstractValidator<CreateOrderItemCommand>
	{
		private readonly IOrderAsyncRepository _orderAsyncRepository;

		public CreateOrderItemValidator(IOrderAsyncRepository orderAsyncRepository)
		{
			this._orderAsyncRepository = orderAsyncRepository;

			RuleFor(c => c.OrderItemToCreate.Quantity)
				.GreaterThan(0).WithMessage("Quantity must be greater than 0");
			
			RuleFor(c => c.OrderItemToCreate.Price)
				.GreaterThan(0).WithMessage("Price must be greater than 0");

			RuleFor(c => c)
				.MustAsync(OrderExists).WithMessage("Order must exist");
		}
		
		private async Task<bool> OrderExists(CreateOrderItemCommand command, CancellationToken cancellationToken)
		{
			return (await this._orderAsyncRepository.GetByIdAsync(command.OrderItemToCreate.OrderId)) != null;
		}
	}
}