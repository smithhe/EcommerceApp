using System;

namespace Ecommerce.Shared.Exceptions
{
    public class InvalidPaymentSourceException : ApplicationException
    {
        public InvalidPaymentSourceException(string message) : base(message) 
        {

        }
    }
}