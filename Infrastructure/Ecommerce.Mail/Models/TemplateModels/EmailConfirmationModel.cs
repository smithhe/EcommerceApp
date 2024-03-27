using Ecommerce.Mail.Contracts;

namespace Ecommerce.Mail.Models.TemplateModels
{
    /// <summary>
    /// Model for email confirmation template
    /// </summary>
    public class EmailConfirmationModel : ITemplateModel
    {
        /// <summary>
        /// Name of the user
        /// </summary>
        public string Name { get; set; } = null!;
        
        /// <summary>
        /// Link to confirm email
        /// </summary>
        public string ConfirmationLink { get; set; } = null!;
    }
}