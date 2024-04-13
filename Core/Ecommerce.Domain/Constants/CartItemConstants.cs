namespace Ecommerce.Domain.Constants
{
    public static class CartItemConstants
    {
        public const string _createSuccessMessage = "Successfully added the item to your cart.";
        public const string _createErrorMessage = "Error adding the item to your cart, please try again later.";
        
        public const string _deleteSingleItemSuccessMessage = "Successfully removed the item from your cart.";
        public const string _deleteSingleItemErrorMessage = "Error removing the item from your cart, please try again later.";
        
        public const string _deleteAllItemsSuccessMessage = "Successfully cleared your cart.";
        public const string _deleteAllItemsErrorMessage = "Error clearing your cart, please try again later.";
        
        public const string _updateSuccessMessage = "Successfully updated the item in your cart.";
        public const string _updateErrorMessage = "Error updating the item in your cart, please try again later.";
        
        public const string _getAllItemsSuccessMessage = "Successfully retrieved all items in your cart.";
        public const string _getAllItemsErrorMessage = "Error retrieving all items in your cart, please try again later.";
        
        public const string _genericValidationErrorMessage = "Please fix the listed errors and try again.";
    }
}