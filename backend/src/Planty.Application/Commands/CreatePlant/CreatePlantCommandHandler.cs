namespace Planty.Application.Commands.CreatePlant;

using MediatR;
using Planty.Application.Common;
using Planty.Contracts.Plants;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

public class CreatePlantCommandHandler : IRequestHandler<CreatePlantCommand, PlantResponse>
{
    private readonly IPlantRepository _plantRepository;

    public CreatePlantCommandHandler(IPlantRepository plantRepository)
    {
        _plantRepository = plantRepository;
    }

    public async Task<PlantResponse> Handle(CreatePlantCommand request, CancellationToken cancellationToken)
    {
        var plant = new Plant
        {
            Name = request.Name,
            Species = request.Species,
            Description = request.Description,
            WateringIntervalDays = request.WateringIntervalDays,
            FertilizationIntervalDays = request.FertilizationIntervalDays,
            LocationId = request.LocationId,
            UserId = request.UserId
        };

        var createdPlant = await _plantRepository.AddAsync(plant, cancellationToken);
        await _plantRepository.SaveChangesAsync(cancellationToken);

        return PlantMapper.MapToResponse(createdPlant);
    }
}
