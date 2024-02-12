using Refit;
using HttpMethod = Ecommerce.PayPal.Models.Enums.HttpMethod;

namespace Ecommerce.PayPal.Models
{
    /// <summary>
    /// Represents a link to a resource.
    /// </summary>
    public class Link
    {
        /// <summary>
        /// The complete target URL.
        /// To make the related call, combine the method with this URI Template-formatted link.
        /// For pre-processing, include the $, (, and ) characters.
        /// The href is the key HATEOAS component that links a completed call with a subsequent call.
        /// </summary>
        [AliasAs("href")]
        public string Href { get; set; } = null!;

        /// <summary>
        /// The link relation type, which serves as an ID for a link that unambiguously describes the semantics of the link.
        /// </summary>
        [AliasAs("rel")]
        public string Rel { get; set; } = null!;

        /// <summary>
        /// The HTTP method required to make the related call.
        /// </summary>
        [AliasAs("method")]
        public HttpMethod? Method { get; set; }
    }
}