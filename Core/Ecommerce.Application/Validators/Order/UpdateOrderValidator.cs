using Ecommerce.Application.Features.Order.Commands.UpdateOrder;
using FluentValidation;

namespace Ecommerce.Application.Validators.Order
{
	public class UpdateOrderValidator : AbstractValidator<UpdateOrderCommand>
	{
		public UpdateOrderValidator()
		{
			RuleFor(c => c.OrderToUpdate!.Status)
				.IsInEnum().WithMessage("Must use a predefined status");

			RuleFor(c => c.OrderToUpdate!.Total)
				.GreaterThan(0).WithMessage("Total must be greater than 0");
		}
	}
}