using Ecommerce.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.Persistence.Contracts
{
	/// <summary>
	/// Extends the <see cref="IAsyncRepository"/> interface with an additional method for <see cref="Product"/> entities
	/// </summary>
	public interface IProductAsyncRepository : IAsyncRepository<Product>
	{
		/// <summary>
		/// Retrieves all <see cref="Product"/> entities from the database with the specified <see cref="Category"/> ID.
		/// </summary>
		/// <param name="categoryId">The ID of the <see cref="Category"/> to find all corresponding <see cref="Product"/> entities</param>
		/// <returns>
		/// A <c>IEnumerable</c> of all <see cref="Product"/> entities found;
		/// A empty <c>IEnumerable</c> if none are found.
		/// </returns>
		Task<IEnumerable<Product>> ListAllAsync(int categoryId);
	}
}