using Sdk.Client.NavTiles.Attributes;
using Sdk.Client.NavTiles.Components;
using Sdk.Client.NavTiles.Enums;

namespace Ping.Client.Ping.NavTiles
{
    [InitialNavTile<PingClientModule>(HorizontalSpan = NavTileSpan.Two)]
    public partial class ProgressNavTile : NavTileBase
    {
    }
}
