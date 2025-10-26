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
        private readonly IWateringRepository _wateringRepository;

        public WaterPlantCommandHandler(IPlantRepository plantRepository, IWateringRepository wateringRepository)
        {
            _plantRepository = plantRepository;
            _wateringRepository = wateringRepository;
        }

        public async Task<PlantResponse> Handle(WaterPlantCommand request, CancellationToken cancellationToken)
        {

            var plant = await _plantRepository.GetByIdAsync(request.PlantId, cancellationToken);
            if (plant == null || plant.UserId != request.UserId)
                throw new Exception($"Plant with id {request.PlantId} not found for this user");

            // Create a new watering record
            var watering = new Watering
            {
                PlantId = plant.Id,
                WateredAt = DateTime.UtcNow
            };

            await _wateringRepository.AddAsync(watering, cancellationToken);
            await _wateringRepository.SaveChangesAsync(cancellationToken);

            // Calculate next watering due
            DateTime? nextWateringDue = null;
            if (plant.WateringIntervalDays.HasValue)
            {
                nextWateringDue = watering.WateredAt.AddDays(plant.WateringIntervalDays.Value);
            }

            return new PlantResponse(
                plant.Id,
                plant.Name,
                plant.Species,
                plant.Description,
                plant.DateAdded,
                watering.WateredAt,  // Use the new watering record's timestamp
                plant.WateringIntervalDays,
                plant.Location?.Name,
                plant.ImageUrl,
                nextWateringDue
            );
        }
    }
}