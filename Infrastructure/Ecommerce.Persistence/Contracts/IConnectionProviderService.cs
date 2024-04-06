using System.Data;

namespace Ecommerce.Persistence.Contracts
{
    /// <summary>
    /// A service that provides a connection to the database
    /// </summary>
    public interface IConnectionProviderService
    {
        /// <summary>
        /// Creates a new connection to the database
        /// </summary>
        /// <returns>
        /// A connection to the database
        /// </returns>
        IDbConnection GetConnection();
    }
}