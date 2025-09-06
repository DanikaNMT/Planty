namespace PlantApp.Application.Queries.GetPlantById;

using MediatR;
using PlantApp.Contracts.Plants;

public record GetPlantByIdQuery(Guid Id) : IRequest<PlantResponse?>;