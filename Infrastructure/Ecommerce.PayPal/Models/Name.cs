using System.Text.Json.Serialization;

namespace Ecommerce.PayPal.Models
{
    /// <summary>
    /// Represents a name in PayPal.
    /// </summary>
    public class Name
    {
        /// <summary>
        /// When the party is a person, the party's given, or first, name.
        /// </summary>
        [JsonPropertyName("given_name")]
        public string? GivenName { get; set; }
        
        /// <summary>
        /// When the party is a person, the party's surname or family name.
        /// Also known as the last name.
        /// Required when the party is a person.
        /// Use also to store multiple surnames including the matronymic, or mother's, surname.
        /// </summary>
        [JsonPropertyName("surname")]
        public string? Surname { get; set; }
    }
}