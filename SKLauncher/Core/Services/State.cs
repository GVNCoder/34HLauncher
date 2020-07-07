using System.Collections;

namespace Launcher.Core.Services
{
    public static class State
    {
        private static readonly Hashtable _state = new Hashtable();
        private static readonly object _lock = new object();

        public static Hashtable Storage
        {
            get
            {
                lock (_lock)
                {
                    return _state;
                }
            }
        }
    }
}