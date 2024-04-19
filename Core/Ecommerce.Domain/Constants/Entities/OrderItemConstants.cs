namespace Ecommerce.Domain.Constants.Entities
{
    public static class OrderItemConstants
    {
        public const string _createUserNameErrorMessage = "Username cannot be null or empty";
        public const string _createSqlErrorMessage = "Sql failed to create OrderItem";
        
        public const string _getAllOrderItemsByOrderIdErrorMessage = "No OrderItems Found";
        
        public const string _genericValidationErrorMessage = "Validation failed";
    }
}