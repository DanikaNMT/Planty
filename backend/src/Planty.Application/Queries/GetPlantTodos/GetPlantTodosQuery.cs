namespace Planty.Application.Queries.GetPlantTodos;

using MediatR;
using Planty.Contracts.Plants;

public record GetPlantTodosQuery(Guid UserId, int HoursAhead = 24) : IRequest<IEnumerable<PlantTodoResponse>>;
