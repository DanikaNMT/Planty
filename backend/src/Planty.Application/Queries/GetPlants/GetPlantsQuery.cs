namespace PlantApp.Application.Queries.GetPlants;

using MediatR;
using PlantApp.Contracts.Plants;

public record GetPlantsQuery : IRequest<IEnumerable<PlantResponse>>;