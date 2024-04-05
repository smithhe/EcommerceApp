// ReSharper disable InconsistentNaming
namespace Ecommerce.PayPal.Models.Enums
{
    public class OrderStatus
    {
        public const string CREATED = "CREATED";
        public const string SAVED = "SAVED";
        public const string APPROVED = "APPROVED";
        public const string VOIDED = "VOIDED";
        public const string COMPLETED = "COMPLETED";
        public const string PAYER_ACTION_REQUIRED = "PAYER_ACTION_REQUIRED";
    }
}