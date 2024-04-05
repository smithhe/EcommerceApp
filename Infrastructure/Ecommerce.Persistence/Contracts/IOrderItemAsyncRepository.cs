using Ecommerce.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.Persistence.Contracts
{
	/// <summary>
	/// Extends the <see cref="T:Ecommerce.Persistence.Contracts.IAsyncRepository`1"/> interface with an additional method for <see cref="OrderItem"/> entities
	/// </summary>
	public interface IOrderItemAsyncRepository : IAsyncRepository<OrderItem>
	{
		/// <summary>
		/// Retrieves all <see cref="OrderItem"/> entities from the database with the specified <see cref="Order"/> ID.
		/// </summary>
		/// <param name="orderId">The ID of the <see cref="Order"/> to find all corresponding <see cref="OrderItem"/> entities</param>
		/// <returns>
		/// A <c>IEnumerable</c> of all <see cref="OrderItem"/> entities found;
		/// A empty <c>IEnumerable</c> if none are found.
		/// </returns>
		Task<IEnumerable<OrderItem>> ListAllAsync(int orderId);
	}
}