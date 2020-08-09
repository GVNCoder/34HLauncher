using System;
using System.Collections;
using System.Windows;
using Launcher.Core.Data.Model.Event;

namespace Launcher.Core.Service
{
    /// <summary>
    /// Defines the mechanism of data flows in the application, as well as saving its state.
    /// </summary>
    public interface IApplicationState
    {
        /// <summary>
        /// Gets current application instance.
        /// </summary>
        Application Application { get; }
        /// <summary>
        /// Gets application state data.
        /// </summary>
        Hashtable State { get; }

        /// <summary>
        /// Occurs, when called <see cref="IApplicationState.SetState"/> method with raiseEvent param = true.
        /// </summary>
        event EventHandler<ApplicationStateEventArgs> StateChanged;
        /// <summary>
        /// Occurs, when called <see cref="IApplicationState.RegisterState"/> method with raiseEvent param = true.
        /// </summary>
        event EventHandler<ApplicationStateEventArgs> StateRegistered;

        /// <summary>
        /// Sets specified state value.
        /// </summary>
        /// <param name="key">A state key.</param>
        /// <param name="state">A state value.</param>
        /// <param name="raiseEvent">Indicates whether the event <see cref="IApplicationState.StateChanged"/> should be raised.</param>
        void SetState(string key, object state, bool raiseEvent = true);
        /// <summary>
        /// Registers a new state.
        /// </summary>
        /// <param name="key">A state key.</param>
        /// <param name="initialValue">A state initial value.</param>
        /// <param name="raiseEvent">Indicates whether the event <see cref="IApplicationState.StateRegistered"/> should be raised.</param>
        void RegisterState(string key, object initialValue, bool raiseEvent = false);
        /// <summary>
        /// Finds and returns a casted state value.
        /// </summary>
        /// <param name="key">A target state key.</param>
        /// <param name="defaultValue">A standard value that will be returned in case of some failure.</param>
        /// <typeparam name="TCast">Desired type.</typeparam>
        /// <returns>Casted state value.</returns>
        TCast GetState<TCast>(string key, TCast defaultValue);
    }
}