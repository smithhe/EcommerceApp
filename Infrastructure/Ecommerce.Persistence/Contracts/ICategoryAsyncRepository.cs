using Ecommerce.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.Persistence.Contracts
{
	/// <summary>
	/// Extends the <see cref="IAsyncRepository"/> interface with an additional method for <see cref="Category"/> entities
	/// </summary>
	public interface ICategoryAsyncRepository : IAsyncRepository<Category>
	{
		/// <summary>
		/// Retrieves all <see cref="Category"/> rows from the database.
		/// </summary>
		/// <returns>
		/// A <c>IEnumerable</c> of all <see cref="Category"/> entities found;
		/// A empty <c>IEnumerable</c> if none are found.
		/// </returns>
		Task<IEnumerable<Category>> ListAllAsync();

		/// <summary>
		/// Checks the table to see if the Name of a <see cref="Category"/> already exists
		/// </summary>
		/// <param name="name">The name to check for</param>
		/// <returns>
		/// <c>false</c> if found;
		/// <c>true</c> if not found
		/// </returns>
		Task<bool> IsNameUnique(string name);
	}
}