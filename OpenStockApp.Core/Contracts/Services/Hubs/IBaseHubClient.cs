using Microsoft.AspNetCore.SignalR.Client;

namespace OpenStockApp.Core.Contracts.Services.Hubs;

public interface IBaseHubClient
{
    public Task StartAsync(CancellationToken cancellationToken = default);
    public Task StopAsync(CancellationToken cancellationToken = default);

    public event EventHandler? Connected;
    public event EventHandler<Exception?>? Closed;
    public event EventHandler? Reconnecting;
    public event EventHandler? Reconnected;

    public string? ConnectionId { get; }
    public HubConnectionState State { get; }

}
