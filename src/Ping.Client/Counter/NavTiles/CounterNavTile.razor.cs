using Sdk.Client.NavTiles.Attributes;
using Sdk.Client.NavTiles.Components;

namespace Ping.Client.Counter.NavTiles
{
    [InitialNavTile<PingClientModule>(LinkTarget = PingClientModule.CounterPageRoute)]
    public partial class CounterNavTile : NavTileBase
    {
    }
}
