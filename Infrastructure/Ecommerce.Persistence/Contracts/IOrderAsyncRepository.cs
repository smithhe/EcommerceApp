using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.Persistence.Contracts
{
	/// <summary>
	/// Extends the <see cref="T:Ecommerce.Persistence.Contracts.IAsyncRepository`1"/> interface with an additional method for <see cref="Order"/> entities
	/// </summary>
	public interface IOrderAsyncRepository : IAsyncRepository<Order>
	{
		/// <summary>
		/// Retrieves all <see cref="Order"/> entities from the database with the specified <see cref="EcommerceUser"/> ID.
		/// </summary>
		/// <param name="userId">The ID of the <see cref="EcommerceUser"/> to find all corresponding <see cref="Order"/> entities</param>
		/// <returns>
		/// A <c>IEnumerable</c> of all <see cref="Order"/> entities found;
		/// A empty <c>IEnumerable</c> if none are found.
		/// <c>null</c> if an error occurs.
		/// </returns>
		Task<IEnumerable<Order>?> ListAllAsync(Guid userId);
	}
}