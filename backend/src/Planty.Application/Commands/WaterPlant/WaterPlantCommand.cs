using MediatR;
using System;
using Planty.Contracts.Plants;

namespace Planty.Application.Commands.WaterPlant
{
    public record WaterPlantCommand(Guid PlantId, Guid UserId) : IRequest<PlantResponse>;
}