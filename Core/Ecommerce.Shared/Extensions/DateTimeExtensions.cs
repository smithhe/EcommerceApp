using System;
using System.Runtime.InteropServices;

namespace Ecommerce.Shared.Extensions
{
	public static class DateTimeExtensions
	{
		public static DateTime StartOfNextDay(this DateTime date, int startHour = 0)
		{
			DateTime nextDay = date.AddDays(1);
			return new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 0, 0, 0);
		}
		
		public static DateTime ToEst(this DateTime date)
		{
			// Determine the time zone ID based on the operating system.
			string timeZoneId;
			
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				timeZoneId = "Eastern Standard Time"; // Windows time zone ID
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				timeZoneId = "America/New_York"; // IANA time zone ID for Eastern Time
			}
			else
			{
				throw new PlatformNotSupportedException("Unsupported operating system");
			}

			// Get the TimeZoneInfo object based on the time zone ID.
			TimeZoneInfo easternZone;
			try
			{
				easternZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
			}
			catch (TimeZoneNotFoundException)
			{
				Console.WriteLine($"The time zone ID '{timeZoneId}' could not be found.");
				throw;
			}
			catch (InvalidTimeZoneException)
			{
				Console.WriteLine($"The time zone ID '{timeZoneId}' is invalid.");
				throw;
			}

			// Convert the provided UTC date to Eastern Time.
			return TimeZoneInfo.ConvertTimeFromUtc(date, easternZone);
		}
	}
}