using System;
using System.Threading.Tasks;

namespace Ecommerce.FastEndpoints.Contracts
{
    /// <summary>
    /// A service that handles operations on the JWT token passed in HTTP requests
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Validates if the JWT token passed in the HTTP request is still valid for use
        /// </summary>
        /// <param name="token">The JWT token passed in the HTTP request</param>
        /// <returns></returns>
        Task<bool> ValidateTokenAsync(string? token);

        /// <summary>
        /// Retrieves the username from the token
        /// </summary>
        /// <param name="token">The JWT token passed in the HTTP request</param>
        /// <returns>
        /// A string containing the username if found;
        /// <c>null</c> if token is null or no claim is found for Name
        /// </returns>
        string? GetUserNameFromToken(string? token);

        /// <summary>
        /// Retrieves the user id from the auth token
        /// </summary>
        /// <param name="token">The auth token to parse</param>
        /// <returns>
        /// The user id if found;
        /// <c>null</c> if token is null or no claim is found for NameIdentifier
        /// </returns>
        Guid? GetUserIdFromToken(string? token);
    }
}