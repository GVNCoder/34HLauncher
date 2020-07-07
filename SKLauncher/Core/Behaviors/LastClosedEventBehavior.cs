using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;

using Interaction_ = System.Windows.Interactivity.Interaction;
using EventTrigger = System.Windows.Interactivity.EventTrigger;

namespace Launcher.Core.Behaviors
{
    public class LastClosedEventBehavior : Behavior<Window>
    {
        private InvokeCommandAction _commandAction;

        protected override void OnAttached()
        {
            var triggers = Interaction_
                .GetTriggers(AssociatedObject)
                .OfType<EventTrigger>();
            var actions = triggers
                .First(t => t.EventName == "Closing")
                .Actions
                .OfType<InvokeCommandAction>();

            _commandAction = actions.First();
            AssociatedObject.Closing += _windowClosingHandler;
        }

        private void _windowClosingHandler(object sender, CancelEventArgs e)
        {
            _commandAction.CommandParameter = e;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Closing -= _windowClosingHandler;
        }
    }
}