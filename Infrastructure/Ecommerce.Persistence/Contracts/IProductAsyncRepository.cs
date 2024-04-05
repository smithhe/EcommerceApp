using Ecommerce.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.Persistence.Contracts
{
	/// <summary>
	/// Extends the <see cref="T:Ecommerce.Persistence.Contracts.IAsyncRepository`1"/> interface with an additional method for <see cref="Product"/> entities
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
		
		/// <summary>
		/// Checks the table to see if the Name of a <see cref="Product"/> already exists
		/// </summary>
		/// <param name="name">The name to check for</param>
		/// <returns>
		/// <c>false</c> if found;
		/// <c>true</c> if not found
		/// </returns>
		Task<bool> IsNameUnique(string name);

		/// <summary>
		/// Retrieves the id of the <see cref="Category"/> for the <see cref="Product"/> 
		/// </summary>
		/// <param name="id">The unique identifier of the <see cref="Product"/></param>
		/// <returns>
		/// The Id of the <see cref="Category"/> if found; -1 if not found
		/// </returns>
		Task<int> GetCategoryId(int id);
	}
}