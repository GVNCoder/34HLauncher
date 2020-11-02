using System;
using System.Windows.Input;
using Launcher.Core.Service.Base;

namespace Launcher.Core.Dialog
{
    public class BaseDialogViewModel : BaseControlViewModel
    {
        public override ICommand LoadedCommand => throw new NotImplementedException();
        public override ICommand UnloadedCommand => throw new NotImplementedException();

        public Dialog Dialog { get; set; }
    }
}