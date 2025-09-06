namespace Planty.Application.Queries.GetPlantById;

using MediatR;
using Planty.Contracts.Plants;

public record GetPlantByIdQuery(Guid Id) : IRequest<PlantResponse?>;