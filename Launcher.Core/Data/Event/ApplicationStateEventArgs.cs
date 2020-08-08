using System;
using Launcher.Core.Service;

namespace Launcher.Core.Data.Event
{
    /// <summary>
    /// Defines an argument for <see cref="IApplicationState"/> events
    /// </summary>
    public class ApplicationStateEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the state.
        /// </summary>
        public object State { get; }
        /// <summary>
        /// Gets the state key.
        /// </summary>
        public string Key { get; }
        /// <summary>
        /// Cast state values to a given type.
        /// </summary>
        /// <typeparam name="T">Desired type.</typeparam>
        /// <returns>Casted state value.</returns>
        public T Cast<T>() => (T) State;

        public ApplicationStateEventArgs(string key, object state)
        {
            Key = key;
            State = state;
        }
    }
}