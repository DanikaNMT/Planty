namespace Planty.Contracts.Plants;

public record FertilizationResponse(
    Guid Id,
    DateTime FertilizedAt,
    string? Notes
);
