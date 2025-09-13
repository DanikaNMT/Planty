using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Planty.Contracts.Plants;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

namespace Planty.Application.Commands.WaterPlant
{
    public class WaterPlantCommandHandler : IRequestHandler<WaterPlantCommand, PlantResponse>
    {
        private readonly IPlantRepository _plantRepository;

        public WaterPlantCommandHandler(IPlantRepository plantRepository)
        {
            _plantRepository = plantRepository;
        }

        public async Task<PlantResponse> Handle(WaterPlantCommand request, CancellationToken cancellationToken)
        {
            var plant = await _plantRepository.GetByIdAsync(request.PlantId, cancellationToken);
            if (plant == null)
                throw new Exception($"Plant with id {request.PlantId} not found");

            plant.LastWatered = DateTime.UtcNow;
            await _plantRepository.UpdateAsync(plant, cancellationToken);
            await _plantRepository.SaveChangesAsync(cancellationToken);

            var nextWateringDue = plant.LastWatered?.AddDays(plant.WateringIntervalDays) ?? plant.DateAdded.AddDays(plant.WateringIntervalDays);

            return new PlantResponse(
                plant.Id,
                plant.Name,
                plant.Species,
                plant.Description,
                plant.DateAdded,
                plant.LastWatered,
                plant.WateringIntervalDays,
                plant.Location,
                plant.ImageUrl,
                nextWateringDue
            );
        }
    }
}