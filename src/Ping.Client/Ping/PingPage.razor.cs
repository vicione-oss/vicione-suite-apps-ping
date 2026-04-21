using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Ping.Client.Ping.Services;
using Ping.Public.Commands;
using Ping.Public.Contracts;
using Ping.Public.Events;
using Ping.Public.Requests;
using Sdk.Client.Infrastructure;
using Sdk.Utils;
using ViciOne.Ui.Blazor.Components.Grid.Services;

namespace Ping.Client.Ping;

public sealed partial class PingPage :
    IEventConsumer<PingCreated>,
    IEventConsumer<PingActivity>,
    IEventConsumer<PingDeleted>,
    IEventConsumer<PingError>
{
    private readonly Dictionary<Guid, Guid> _triggeredDelete = [];
    private readonly Dictionary<Guid, Guid> _triggeredCreate = [];
    private readonly AutoDisposeList<IDisposable> _subscriptionHandle = [];
    private bool _isSinglePingSelected;
    private IQueryable<PingResult>? _pingsQueryable;

    [Inject] private IUiMediator Mediator { get; set; } = default!;
    [Inject(Key = typeof(PingPageServiceKey))] private IGridItemSelection<Guid> PingGridItemSelection { get; set; } = default!;
    [Inject] private PingState PingState { get; set; } = default!;

    private int CreateCommandDelayMs { get; set; } = 10 * 1000;// sek * ms
    private string? LastActivity { get; set; }
    private string? LastError { get; set; }
    protected override string PageTitle => Localization.Common.ModuleTitle;
    private string? SelectedIdString { get; set; }
    private bool ThrowOnConsume { get; set; }
    private bool ThrowOnRetry { get; set; }

    public async Task Consume(ClientContext<PingCreated> context, CancellationToken cancellationToken)
    {
        if (IsCorrelatedCreate(context.Message.PingId, context.CorrelationId))
        {
            // we triggered this ping creation ourself
            Logger.LogInformation("Handle {Name} with pingId {PingId} created by you", nameof(PingCreated), context.Message.PingId);
        }
        else
        {
            // this was triggered by someone else
            Logger.LogInformation("Handle {Name} with pingId {PingId} created by someone. correlationId: {CorrelationId}",
                nameof(PingCreated),
                context.Message.PingId,
                context.CorrelationId);
        }

        _triggeredCreate.Remove(context.Message.PingId);

        if (PingState.AddToStore(context.Message))
        {
            UpdatePingsQueryable();
            await InvokeAsync(StateHasChanged);
        }
    }

    public async Task Consume(ClientContext<PingDeleted> context, CancellationToken cancellationToken)
    {
        // the correlation id matches so this event was triggered by our command
        if (IsCorrelatedDelete(context.Message.PingId, context.CorrelationId))
        {
            // the correlation id matches so this event was triggered by our command
            Logger.LogInformation("Consume {Name} with pingId {PingId} triggered by you",
                nameof(PingDeleted),
                context.Message.PingId);
        }
        else
        {
            // this was triggered by someone else
            Logger.LogInformation("Consume {Name} with pingId: {PingId} created by someone. correlationId: {CorrelationId}",
                nameof(PingDeleted),
                context.Message.PingId,
                context.CorrelationId);
        }

        _triggeredDelete.Remove(context.Message.PingId);

        if (PingState.RemoveFromStore(context.Message.PingId))
        {
            UpdatePingsQueryable();
            PingGridItemSelection.Remove(context.Message.PingId);
            await InvokeAsync(StateHasChanged);
        }
    }

    public async Task Consume(ClientContext<PingError> context, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Consume {MessageType}. pingId={PingId} code={Code} error={Error}",
            context.Message.GetType().Name,
            context.Message.PingId,
            context.Message.Error.ErrorCode,
            context.Message.Error.Message);

        switch (context.Message.Error.ErrorCode)
        {
            case PingError.CreateFailed:
                _triggeredCreate.Remove(context.Message.PingId ?? Guid.Empty);
                break;

            case PingError.DeleteFailed:
            case PingError.DeleteFailedNotFound:
                _triggeredDelete.Remove(context.Message.PingId ?? Guid.Empty);
                break;
        }

        LastActivity = null;
        LastError = $"Error occured:{Environment.NewLine}{context.Message.Error.Message}";
        await InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// this event was published by a backend activity
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    public async Task Consume(ClientContext<PingActivity> context, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Consume {ErrorType}. pingId={PingId} activity={Activity}",
            context.Message.GetType().Name,
            context.Message.PingId,
            context.Message.Activity);

        LastActivity = $"{context.Message.Activity} on ping {context.Message.PingId} correlated by {context.CorrelationId}";
        await InvokeAsync(StateHasChanged);
    }

    private async Task Create()
    {
        var pingId = Guid.NewGuid();

        try
        {
            Logger.LogInformation("Creating ping {PingId}", pingId);

            LastError = null;

            var command = new CreatePing(pingId, 0)
            {
                ThrowOnConsume = ThrowOnConsume,
                ThrowOnRetry = ThrowOnRetry
            };
            await Mediator.Send(command);

            _triggeredCreate.Add(pingId, command.CorrelationId);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "{Call} failed on ping {PingId}", nameof(Create), pingId);
        }
    }

    private async Task CreateDelayed()
    {
        var pingId = Guid.NewGuid();

        Logger.LogInformation("Creating ping {PingId} delayed by {Ms}ms", pingId, CreateCommandDelayMs);

        LastError = null;
        var command = new CreatePing(pingId, CreateCommandDelayMs)
        {
            ThrowOnConsume = ThrowOnConsume,
            ThrowOnRetry = ThrowOnRetry
        };
        await Mediator.Send(command);

        _triggeredCreate.Add(pingId, command.CorrelationId);
    }

    private async Task Delete()
    {
        var pingId = PingGridItemSelection.FirstOrDefault();

        var toDelete = PingState.StoredPings?.SingleOrDefault(p => p.Id == pingId);
        if (toDelete is not null)
        {
            LastError = null;
            var command = new RemovePing(pingId)
            {
                ThrowOnConsume = ThrowOnConsume,
                ThrowOnRetry = ThrowOnRetry
            };
            await Mediator.Send(command);

            Logger.LogInformation("Deleting ping {PingId} (CorrelationId={CorrelationId})", pingId, command.CorrelationId);

            _triggeredDelete.Add(pingId, command.CorrelationId);
        }
    }

    protected override ValueTask DisposeInternal()
    {
        _subscriptionHandle.Dispose();
        return base.DisposeInternal();
    }

    private bool IsCorrelatedCreate(Guid pingId, Guid? correlationId)
        => _triggeredCreate.TryGetValue(pingId, out var info) &&
            Equals(info, correlationId);

    private bool IsCorrelatedDelete(Guid pingId, Guid? correlationId)
        => _triggeredDelete.TryGetValue(pingId, out var info) &&
            Equals(info, correlationId);

    protected override async Task OnInitializedAsync()
    {
        await RequestAll();

        _subscriptionHandle.Add(Mediator.Register<PingCreated>(this));
        _subscriptionHandle.Add(Mediator.Register<PingActivity>(this));
        _subscriptionHandle.Add(Mediator.Register<PingDeleted>(this));
        _subscriptionHandle.Add(Mediator.Register<PingError>(this));

        UpdatePingsQueryable();
        PingGridItemSelection.Changed += PingGridItemSelectionChanged;
    }

    private async void PingGridItemSelectionChanged(GridItemSelectionChangedEventArgs<Guid> obj)
    {
        if (UpdateStatesDependingOnInstanceGridItemSelection())
            await InvokeAsync(StateHasChanged);
    }

    private async Task RequestAll()
    {
        try
        {
            var data = await Mediator.Request<GetPingsRequest, GetPingsResponse>(new(null)
            {
                ThrowOnConsume = ThrowOnConsume,
            });

            PingState.StoredPings = data.Result;
            SelectedIdString = PingState.GetFirstIdString();

            Logger.LogInformation("{Response} contains {Count} pings", nameof(GetPingsResponse), PingState.StoredPings?.Count);
        }
        catch (Exception e)
        {
            Logger.LogError(e, nameof(RequestAll));
        }
    }

    private async Task RequestSelected()
    {
        try
        {
            if (string.IsNullOrEmpty(SelectedIdString))
                return;

            var pingId = Guid.Parse(SelectedIdString);
            var response = await Mediator.Request<GetPingsRequest, GetPingsResponse>(new(pingId)
            {
                ThrowOnConsume = ThrowOnConsume,
            });

            PingState.SelectedPing = response.Result.FirstOrDefault();

            Logger.LogInformation("{Call}({PingId}) contains {Count} pings", nameof(GetPingsResponse), pingId, PingState.StoredPings?.Count);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "{Call} pingId: {PingId}", nameof(RequestSelected), SelectedIdString);
        }
    }

    private void UpdatePingsQueryable()
        => _pingsQueryable = PingState.StoredPings?.AsQueryable();

    private bool UpdateStatesDependingOnInstanceGridItemSelection()
    {
        var somethingChanged = false;

        var isSingleInstanceSelected = PingGridItemSelection.Count == 1;
        if (isSingleInstanceSelected != _isSinglePingSelected)
        {
            _isSinglePingSelected = isSingleInstanceSelected;

            somethingChanged = true;
        }

        return somethingChanged;
    }
}
