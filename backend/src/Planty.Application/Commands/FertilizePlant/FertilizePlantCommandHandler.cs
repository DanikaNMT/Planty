using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Planty.Application.Common;
using Planty.Contracts.Plants;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

namespace Planty.Application.Commands.FertilizePlant
{
    public class FertilizePlantCommandHandler : CareActionHandler<Fertilization>, IRequestHandler<FertilizePlantCommand, PlantResponse>
    {
        private readonly IPlantRepository _plantRepository;
        private readonly IFertilizationRepository _fertilizationRepository;

        public FertilizePlantCommandHandler(IPlantRepository plantRepository, IFertilizationRepository fertilizationRepository)
            : base(plantRepository)
        {
            _plantRepository = plantRepository;
            _fertilizationRepository = fertilizationRepository;
        }

        public async Task<PlantResponse> Handle(FertilizePlantCommand request, CancellationToken cancellationToken)
        {
            var plant = await ValidatePlantOwnershipAsync(request.PlantId, request.UserId, cancellationToken);

            // Create a new fertilization record
            var fertilization = new Fertilization
            {
                PlantId = plant.Id,
                FertilizedAt = DateTime.UtcNow
            };

            await _fertilizationRepository.AddAsync(fertilization, cancellationToken);
            await _fertilizationRepository.SaveChangesAsync(cancellationToken);

            // Manually update the plant's Fertilizations collection to include the new fertilization
            // This ensures PlantMapper gets the latest data
            plant.Fertilizations.Add(fertilization);

            return PlantMapper.MapToResponse(plant);
        }
    }
}
