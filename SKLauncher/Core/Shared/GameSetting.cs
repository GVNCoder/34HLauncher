using System;
using System.Collections.Generic;
using Zlo4NET.Api.Models.Shared;

namespace Launcher.Core.Shared
{
    [Serializable]
    public class GameSetting
    {
        public ZGame Game { get; set; }
        public ZGameArchitecture PreferredArchitecture { get; set; }
        public List<string> Dlls { get; set; } = new List<string>();
    }
}