using System;

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
			TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
			return TimeZoneInfo.ConvertTimeFromUtc(date, easternZone);
		}
	}
}