namespace Degen.Application.MarketData.Dtos;

public record AssetDto(Guid Id, string Symbol, string Name, string Type, int Decimals);
