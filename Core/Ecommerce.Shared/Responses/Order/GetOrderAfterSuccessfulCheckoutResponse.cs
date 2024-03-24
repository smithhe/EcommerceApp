using Ecommerce.Shared.Dtos;
using Ecommerce.Shared.Requests.Order;

namespace Ecommerce.Shared.Responses.Order
{
    /// <summary>
    /// A implementation of <see cref="BaseResponse" /> for a request to get a Order after a successful checkout
    /// </summary>
    public class GetOrderAfterSuccessfulCheckoutResponse : BaseResponse
    {
        /// <summary>
        /// The Order with the Id from the <see cref="GetOrderAfterSuccessfulCheckoutApiRequest"/> if it exists
        /// </summary>
        public OrderDto? Order { get; set; }
    }
}