namespace Ecommerce.Shared.Requests.Order
{
    /// <summary>
    /// An Api request to get an Order after a successful checkout
    /// </summary>
    public class GetOrderAfterSuccessfulCheckoutApiRequest
    {
        /// <summary>
        /// The unique identifier of the Order to retrieve
        /// </summary>
        public int Id { get; set; }
    }
}