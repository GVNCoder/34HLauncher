using System;

using Zlo4NET.Api.Models.Shared;

namespace Launcher.Core.Shared
{
    [Serializable]
    public class GameSettings
    {
        public ZGame DataGame                     { get; set; }
        public ZGameArchitecture DataArchitecture { get; set; }

        public string[] DataCollectionInjectableDlls { get; set; }
    }
}