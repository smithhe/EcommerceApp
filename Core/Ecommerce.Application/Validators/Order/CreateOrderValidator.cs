using Ecommerce.Application.Features.Order.Commands.CreateOrder;
using Ecommerce.Shared.Enums;
using FluentValidation;

namespace Ecommerce.Application.Validators.Order
{
	public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
	{
		public CreateOrderValidator()
		{
			RuleFor(c => c.OrderToCreate!.Status)
				.Equal(OrderStatus.Pending).WithMessage("New Orders must have a Pending status");

			RuleFor(c => c.OrderToCreate!.Total)
				.GreaterThan(0).WithMessage("Total must be greater than 0");
			
			//TODO: Rule for User must Exist
		}
	}
}