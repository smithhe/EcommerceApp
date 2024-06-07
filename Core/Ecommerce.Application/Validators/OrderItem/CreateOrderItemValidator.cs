using Ecommerce.Application.Features.OrderItem.Commands.CreateOrderItem;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using Ecommerce.Application.Features.Order.Queries.GetOrderById;
using MediatR;

namespace Ecommerce.Application.Validators.OrderItem
{
	public class CreateOrderItemValidator : AbstractValidator<CreateOrderItemCommand>
	{
		private readonly IMediator _mediator;

		public CreateOrderItemValidator(IMediator mediator)
		{
			this._mediator = mediator;

			RuleFor(c => c.OrderItemToCreate.Quantity)
				.GreaterThan(0).WithMessage("Quantity must be greater than 0");
			
			RuleFor(c => c.OrderItemToCreate.Price)
				.GreaterThan(0).WithMessage("Price must be greater than 0");

			RuleFor(c => c)
				.MustAsync(OrderExists).WithMessage("Order must exist");
		}
		
		private async Task<bool> OrderExists(CreateOrderItemCommand command, CancellationToken cancellationToken)
		{
			return (await this._mediator.Send(new GetOrderByIdQuery { Id = command.OrderItemToCreate.OrderId, FetchOrderItems = false}, cancellationToken)).Order != null;
		}
	}
}