using System;
using System.Windows.Media;

namespace Launcher.Core.Data.Model.Event
{
    public class VisualContentChangedEventArgs : EventArgs
    {
        public Visual Content { get; }

        public VisualContentChangedEventArgs(Visual content)
        {
            Content = content;
        }
    }
}