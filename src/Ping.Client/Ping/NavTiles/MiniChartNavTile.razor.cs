using Microsoft.AspNetCore.Components;
using Ping.Public.Events;
using Ping.Public.Requests;
using Sdk.Client.Infrastructure;
using Sdk.Client.NavTiles.Attributes;
using Sdk.Client.NavTiles.Components;
using Sdk.Client.NavTiles.Enums;
using Sdk.Utils;

namespace Ping.Client.Ping.NavTiles
{
    [InitialNavTile<PingClientModule>(HorizontalSpan = NavTileSpan.Two)]
    public partial class MiniChartNavTile : NavTileBase, IDisposable, IEventConsumer<PingCreated>, IEventConsumer<PingDeleted>
    {
        private int _storedPings = -1;
        private readonly AutoDisposeList<IDisposable> _subscriptionHandle = [];

        [Inject] private IUiMediator Mediator { get; set; } = null!;

        public async Task Consume(ClientContext<PingCreated> context, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _storedPings);

            await InvokeAsync(StateHasChanged);
        }

        public async Task Consume(ClientContext<PingDeleted> context, CancellationToken cancellationToken)
        {
            Interlocked.Decrement(ref _storedPings);

            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _subscriptionHandle.Dispose();
            }
        }

        private async Task<int> FetchStoredPings()
        {
            var data = await Mediator.Request<GetPingsRequest, GetPingsResponse>(new (null));

            return data.Result.Count;
        }

        protected override async Task OnInitializedAsync()
        {
            _storedPings = await FetchStoredPings();

            _subscriptionHandle.Add(Mediator.Register<PingCreated>(this));
            _subscriptionHandle.Add(Mediator.Register<PingDeleted>(this));
        }
    }
}
