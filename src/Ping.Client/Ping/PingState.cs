using Ping.Public.Contracts;
using Ping.Public.Events;

namespace Ping.Client.Ping;

internal class PingState
{
    public string? PingResultOrigin { get; set; }
    public PingResult? SelectedPing { get; set; }
    public List<PingResult>? StoredPings { get; set; }

    public bool AddToStore(PingCreated pingEvent)
    {
        if (StoredPings is null)
            return false;

        StoredPings.Add(new PingResult
        {
            Id = pingEvent.PingId,
            Created = pingEvent.Created,
            RequestId = pingEvent.RequestId,
            Message = pingEvent.Message
        });
        return true;
    }

    public string? GetFirstIdString() => StoredPings?.Select(p => p.Id.ToString()).FirstOrDefault();

    public bool RemoveFromStore(Guid pingId)
    {
        if (StoredPings is null)
            return false;

        var pingToRemove = StoredPings.SingleOrDefault(p => p.Id == pingId);
        if (pingToRemove is null)
            return false;

        StoredPings.Remove(pingToRemove);
        return true;
    }

    public void SelectPing(Guid pingId) => SelectedPing = StoredPings?.SingleOrDefault(p => p.Id == pingId);

}
