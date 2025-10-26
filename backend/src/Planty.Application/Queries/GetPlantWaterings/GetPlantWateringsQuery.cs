namespace Planty.Application.Queries.GetPlantWaterings;

using MediatR;
using Planty.Contracts.Plants;

public record GetPlantWateringsQuery(
    Guid PlantId,
    Guid UserId
) : IRequest<IEnumerable<WateringResponse>>;
