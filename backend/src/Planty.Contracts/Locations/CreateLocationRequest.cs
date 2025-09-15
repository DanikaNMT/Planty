namespace Planty.Contracts.Locations;

public record CreateLocationRequest(
    string Name,
    string? Description
);