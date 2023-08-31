using System;

namespace Ecommerce.Shared.Exceptions
{
	public class BadRequestException : ApplicationException
	{
		public BadRequestException(string message) : base(message) 
		{

		}
	}
}