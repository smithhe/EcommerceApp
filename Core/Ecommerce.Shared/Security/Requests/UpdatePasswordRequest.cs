namespace Ecommerce.Shared.Security.Requests
{
    /// <summary>
    /// A Api request to update a User's password
    /// </summary>
    public class UpdatePasswordRequest
    {
        /// <summary>
        /// The UserName of the User to update the password for
        /// </summary>
        public string? UserName { get; set; }
    
        /// <summary>
        /// The current password of the User
        /// </summary>
        public string? CurrentPassword { get; set; }
    
        /// <summary>
        /// The new password to use
        /// </summary>
        public string? NewPassword { get; set; }
    }
}