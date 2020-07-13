using Zlo4NET.Api.Models.Server;

namespace Launcher.Styles.PlayerRotation
{
    public class PlayerViewModel
    {
        public PlayerSelectorType Selector { get; }
        public string Name { get; }
        public uint Id { get; }

        public PlayerViewModel(PlayerSelectorType selector, ZPlayer player)
        {
            Selector = selector;
            Name = player.Name;
            Id = player.Id;
        }
    }
}