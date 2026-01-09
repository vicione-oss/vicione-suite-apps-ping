using Sdk.Client.NavTiles.Attributes;
using Sdk.Client.NavTiles.Components;

namespace Ping.Client.Builder.NavTiles
{
    [InitialNavTile<PingClientModule>(LinkTarget = PingClientModule.BuilderPageRoute)]
    public partial class BuilderNavTile : NavTileBase
    {
    }
}
