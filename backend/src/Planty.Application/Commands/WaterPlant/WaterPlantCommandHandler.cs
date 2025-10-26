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
            if (plant == null || plant.UserId != request.UserId)
                throw new Exception($"Plant with id {request.PlantId} not found for this user");

            plant.LastWatered = DateTime.UtcNow;
            await _plantRepository.UpdateAsync(plant, cancellationToken);
            await _plantRepository.SaveChangesAsync(cancellationToken);

            DateTime? nextWateringDue = null;
            if (plant.WateringIntervalDays.HasValue)
            {
                nextWateringDue = plant.LastWatered?.AddDays(plant.WateringIntervalDays.Value) ?? 
                                 plant.DateAdded.AddDays(plant.WateringIntervalDays.Value);
            }

            return new PlantResponse(
                plant.Id,
                plant.Name,
                plant.Species,
                plant.Description,
                plant.DateAdded,
                plant.LastWatered,
                plant.WateringIntervalDays,
                plant.Location?.Name,
                plant.ImageUrl,
                nextWateringDue
            );
        }
    }
}