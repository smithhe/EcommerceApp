using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.Persistence.Contracts
{
	/// <summary>
	/// Extends the <see cref="IAsyncRepository"/> interface with an additional method for <see cref="CartItem"/> entities
	/// </summary>
	public interface ICartItemRepository : IAsyncRepository<CartItem>
	{
		/// <summary>
		/// Retrieves all <see cref="CartItem"/> entities from the database for a <see cref="EcommerceUser"/>.
		/// </summary>
		/// <param name="userId">The unique identifier of the <see cref="EcommerceUser"/> to find the cart items for</param>
		/// <returns>
		/// A <c>IEnumerable</c> of all <see cref="CartItem"/> entities found;
		/// A empty <c>IEnumerable</c> if none are found.
		/// </returns>
		Task<IEnumerable<CartItem>> ListAllAsync(Guid userId);

		/// <summary>
		/// Removes all <see cref="CartItem"/> entities from the database for a <see cref="EcommerceUser"/>.
		/// </summary>
		/// <param name="userId">The unique identifier of the <see cref="EcommerceUser"/> to remove all cart items for</param>
		/// <returns>
		/// <c>true</c> if the DELETE is successful;
		/// <c>false</c> if the DELETE fails or no <see cref="CartItem"/> entities are not found.
		/// </returns>
		Task<bool> RemoveUserCartItems(Guid userId);

		/// <summary>
		/// Checks to see if the user already has a <see cref="CartItem"/> for the <see cref="Product"/>
		/// </summary>
		/// <param name="userId">The unique identifier of the <see cref="EcommerceUser"/></param>
		/// <param name="productId">The unique identifier of the <see cref="Product"/></param>
		/// <returns>
		/// <c>true</c> if the <see cref="CartItem"/> exists;
		/// <c>false</c> no <see cref="CartItem"/> is found.
		/// </returns>
		Task<bool> CartItemExistsForUser(Guid userId, int productId);
	}
}