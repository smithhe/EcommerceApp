using Ecommerce.Application.Features.Order.Commands.CreateOrder;
using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Enums;
using FluentValidation;

namespace Ecommerce.Application.Validators.Order
{
	public class CreateOrderValidator : AbstractValidator<OrderDto>
	{
		public CreateOrderValidator()
		{
			RuleFor(o => o.Status)
				.Equal(OrderStatus.Pending).WithMessage("New Orders must have a Pending status");

			RuleFor(o => o.OrderItems)
				.NotEmpty().WithMessage("Order must have at least one Order Item");
			
			RuleFor(o => o.Total)
				.GreaterThan(0).WithMessage("Total must be greater than 0");
			
			//TODO: Rule for User must Exist
		}
	}
}