namespace Planty.Application.Queries.GetPlantCareHistory;

using MediatR;
using Planty.Contracts.Plants;

public record GetPlantCareHistoryQuery(
    Guid PlantId,
    Guid UserId
) : IRequest<IEnumerable<CareEventResponse>>;
