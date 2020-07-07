using System.Collections.Generic;

namespace Launcher.Helpers
{
    public static class ListHelper
    {
        public static IList<T> Empty<T>() => new List<T>();
    }
}