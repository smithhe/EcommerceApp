namespace Ecommerce.Shared.Enums
{
	public enum OrderStatus
	{
		Created, //Orders that have been created but not yet placed
		Pending, //Orders that have been placed but not yet processed for payment
		Processing, //Orders that are currently being processed for payment
		Cancelled, //Orders that have been cancelled
		Completed, //Orders that have been successfully processed for payment
		Failed //Orders that have failed to process for payment
	}
}