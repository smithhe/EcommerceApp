using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Ecommerce.UI.Extensions
{
    public static class DateTimeWasmExtensions
    {
        public static async Task<DateTime> ToEstAsync(this DateTime date, IJSRuntime jsRuntime)
        {
            // Determine the time zone ID based on the operating system.
            string timeZoneId;
			
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                timeZoneId = "Eastern Standard Time";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                timeZoneId = "America/New_York";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER")))
            {
                timeZoneId = await GetBrowserTimeZoneAsync(jsRuntime);
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

        private static async Task<string> GetBrowserTimeZoneAsync(IJSRuntime jsRuntime)
        {
            try
            {
                // Asynchronous JavaScript interop call to determine the browser's time zone
                return await jsRuntime.InvokeAsync<string>(
                    "eval", "Intl.DateTimeFormat().resolvedOptions().timeZone");
            }
            catch (Exception)
            {
                // Error getting the time zone from the browser, return a default value
                Console.WriteLine("Failed to get browser time zone via JS interop.");
                return "Eastern Standard Time";
            }
        }
    }
}