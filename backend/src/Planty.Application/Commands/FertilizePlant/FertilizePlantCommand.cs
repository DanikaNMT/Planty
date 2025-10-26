using MediatR;
using System;
using Planty.Contracts.Plants;

namespace Planty.Application.Commands.FertilizePlant
{
    public record FertilizePlantCommand(Guid PlantId, Guid UserId) : IRequest<PlantResponse>;
}
