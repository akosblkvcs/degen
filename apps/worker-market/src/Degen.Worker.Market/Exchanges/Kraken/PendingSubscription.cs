namespace Degen.Worker.Market.Exchanges.Kraken;

public class PendingSubscription
{
    public string Channel { get; init; } = default!;
    public string Symbol { get; init; } = default!;
    public TaskCompletionSource<bool> Completion { get; } =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
}
