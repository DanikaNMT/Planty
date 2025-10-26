namespace Planty.Application.Queries.GetPlantWaterings;

using MediatR;
using Planty.Contracts.Plants;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

public class GetPlantWateringsQueryHandler : IRequestHandler<GetPlantWateringsQuery, IEnumerable<WateringResponse>>
{
    private readonly IPlantRepository _plantRepository;
    private readonly IWateringRepository _wateringRepository;

    public GetPlantWateringsQueryHandler(IPlantRepository plantRepository, IWateringRepository wateringRepository)
    {
        _plantRepository = plantRepository;
        _wateringRepository = wateringRepository;
    }

    public async Task<IEnumerable<WateringResponse>> Handle(GetPlantWateringsQuery request, CancellationToken cancellationToken)
    {
        // First verify the plant exists and belongs to the user
        var plant = await _plantRepository.GetByIdAsync(request.PlantId, cancellationToken);
        if (plant == null || plant.UserId != request.UserId)
            return Enumerable.Empty<WateringResponse>();

        var waterings = await _wateringRepository.GetByPlantIdAsync(request.PlantId, cancellationToken);
        return waterings.Select(MapToResponse);
    }

    private static WateringResponse MapToResponse(Watering watering)
    {
        return new WateringResponse(
            watering.Id,
            watering.WateredAt,
            watering.Notes
        );
    }
}
