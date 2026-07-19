namespace Degen.Domain.Instruments;

public class Instrument
{
    public Guid Id { get; set; }
    public required string Symbol { get; set; }
    public required string Name { get; set; }
    public required string AssetType { get; set; }
    public required string UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
