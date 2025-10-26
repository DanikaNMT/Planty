namespace Planty.Application.Queries.GetPlantFertilizations;

using MediatR;
using Planty.Contracts.Plants;

public record GetPlantFertilizationsQuery(
    Guid PlantId,
    Guid UserId
) : IRequest<IEnumerable<FertilizationResponse>>;
