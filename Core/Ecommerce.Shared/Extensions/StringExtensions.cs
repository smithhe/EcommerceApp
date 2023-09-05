namespace Ecommerce.Shared.Extensions
{
	public static class StringExtensions
	{
		public static string LowerAndUpperFirst(this string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return input;
			}

			string lowercased = input.ToLower();
			string firstCharUpper = char.ToUpper(lowercased[0]) + lowercased.Substring(1);

			return firstCharUpper;
		}
	}
}