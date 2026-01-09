using System.Timers;
using Sdk.Client.NavTiles.Attributes;
using Sdk.Client.NavTiles.Components;
using Sdk.Client.NavTiles.Enums;

namespace Ping.Client.Ping.NavTiles
{
    [InitialNavTile<PingClientModule>(HorizontalSpan = NavTileSpan.One)]
    public sealed partial class WildCardsNavTile : NavTileBase, IDisposable
    {
        private const string DefaultSubline = "This tool can help to set up so called wild cards and this is a very long subline showing an ellipsis on overflow";

        private string? _subline = DefaultSubline;
        private readonly System.Timers.Timer _reloadTimer = new(3000) { AutoReset = false };

        public override void Click()
        {
            if (!_reloadTimer.Enabled)
            {
                _reloadTimer.Elapsed -= ReloadTimerElapsed;
                _reloadTimer.Elapsed += ReloadTimerElapsed;

                State.BeginLoading();

                _reloadTimer.Start();
            }
        }

        public void Dispose()
        {
            _reloadTimer.Dispose();

            GC.SuppressFinalize(this);
        }

        private void ReloadTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            State.EndLoading();

            _subline = string.IsNullOrEmpty(_subline) ? DefaultSubline : null;
        }
    }
}
