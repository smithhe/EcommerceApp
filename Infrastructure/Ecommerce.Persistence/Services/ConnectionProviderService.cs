using System.Data;
using Ecommerce.Persistence.Contracts;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Ecommerce.Persistence.Services
{
    /// <summary>
    /// A service that provides a connection to the database
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance used for configuration settings.</param>
    public class ConnectionProviderService(IConfiguration configuration) : IConnectionProviderService
    {
        private const string _connectionStringName = "datastorage";

        /// <summary>
        /// Creates a new connection to the database
        /// </summary>
        /// <returns>
        /// A connection to the database
        /// </returns>
        public IDbConnection GetConnection()
        {
            return new MySqlConnection(configuration.GetConnectionString(_connectionStringName));
        }
    }
}