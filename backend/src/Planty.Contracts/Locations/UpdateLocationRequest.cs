namespace Planty.Contracts.Locations;

public record UpdateLocationRequest(
    string Name,
    string? Description
);
