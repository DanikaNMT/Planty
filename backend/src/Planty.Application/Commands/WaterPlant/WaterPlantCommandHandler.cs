using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Planty.Application.Common;
using Planty.Application.Interfaces;
using Planty.Contracts.Plants;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

namespace Planty.Application.Commands.WaterPlant
{
    public class WaterPlantCommandHandler : CareActionHandler<Watering>, IRequestHandler<WaterPlantCommand, PlantResponse>
    {
        private readonly IPlantRepository _plantRepository;
        private readonly IWateringRepository _wateringRepository;

        public WaterPlantCommandHandler(IPlantRepository plantRepository, IWateringRepository wateringRepository, IPermissionService permissionService)
            : base(plantRepository, permissionService)
        {
            _plantRepository = plantRepository;
            _wateringRepository = wateringRepository;
        }

        public async Task<PlantResponse> Handle(WaterPlantCommand request, CancellationToken cancellationToken)
        {

            var plant = await ValidatePlantCarePermissionAsync(request.PlantId, request.UserId, cancellationToken);

            // Create a new watering record
            var watering = new Watering
            {
                PlantId = plant.Id,
                WateredAt = DateTime.UtcNow,
                UserId = request.UserId
            };

            await _wateringRepository.AddAsync(watering, cancellationToken);
            await _wateringRepository.SaveChangesAsync(cancellationToken);

            // Manually update the plant's Waterings collection to include the new watering
            // This ensures PlantMapper gets the latest data
            plant.Waterings.Add(watering);

            return PlantMapper.MapToResponse(plant);
        }
    }
}