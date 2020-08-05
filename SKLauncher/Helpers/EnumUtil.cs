using System;

namespace Launcher.Helpers
{
    internal static class EnumUtil
    {
        public static TEnum Parse<TEnum> (string source) => (TEnum) Enum.Parse(typeof(TEnum), source);
    }
}