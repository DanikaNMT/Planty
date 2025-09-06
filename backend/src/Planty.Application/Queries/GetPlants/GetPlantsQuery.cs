namespace Planty.Application.Queries.GetPlants;

using MediatR;
using Planty.Contracts.Plants;

public record GetPlantsQuery : IRequest<IEnumerable<PlantResponse>>;