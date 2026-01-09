using Sdk.Client.NavTiles.Attributes;
using Sdk.Client.NavTiles.Components;

namespace Ping.Client.Ping.NavTiles
{
    [InitialNavTile<PingClientModule>(LinkTarget = PingClientModule.PingPageRoute)]
    public partial class PingNavTile : NavTileBase
    {
    }
}
