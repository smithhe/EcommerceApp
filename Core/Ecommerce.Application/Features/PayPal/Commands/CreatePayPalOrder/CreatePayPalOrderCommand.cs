using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Responses.PayPal;
using MediatR;

namespace Ecommerce.Application.Features.PayPal.Commands.CreatePayPalOrder
{
    public class CreatePayPalOrderCommand : IRequest<CreatePayPalOrderResponse>
    {
        public OrderDto Order { get; set; } = null!;
    }
}