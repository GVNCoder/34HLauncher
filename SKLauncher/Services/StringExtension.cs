using System;
using System.Linq;

namespace Launcher.Services
{
    public static class StringExtension
    {
        public static bool ContainsIgnoreCase(this string source, string value)
        {
            return source.IndexOf(value, StringComparison.OrdinalIgnoreCase) != -1;
        }

        public static bool IsNumber(this string source) => source.Any(char.IsDigit);
    }
}