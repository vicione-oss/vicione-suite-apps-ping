using Microsoft.AspNetCore.Components;
using Sdk.Client.NavTiles.Attributes;
using Sdk.Client.NavTiles.Components;
using Sdk.Client.NavTiles.Enums;
using Sdk.Client.NavTiles.Services;

namespace Ping.Client.Ping.NavTiles
{
    [InitialNavTile<PingClientModule>(HorizontalSpan = NavTileSpan.One)]
    public partial class TogglerNavTile : NavTileBase
    {
        private readonly string _textNavTileId = "f1080f6b-50fe-49fb-b2cd-7903e2b2779c";

        private string? _subline;

        [Inject] private INavTileRegistry<PingClientModule> NavTileRegistry { get; set; } = default!;

        public override void Click()
        {
            var navTileRegistryItem = NavTileRegistry.FirstOrDefault(i => i.Id == _textNavTileId);

            if (navTileRegistryItem is not null)
                NavTileRegistry.Remove(navTileRegistryItem.Id);
            else
                NavTileRegistry.Add<TextNavTile>(_textNavTileId, linkTarget: PingClientModule.PingPageRoute);

            UpdateSubline();
        }

        protected override void OnInitialized() => UpdateSubline();

        private void UpdateSubline()
        {
            _subline = NavTileRegistry.Any(i => i.Id == _textNavTileId)
                ? "Click here to remove additional tile"
                : "Click here to add additional tile";
        }
    }
}
