namespace Planty.Application.Queries.GetPlantCareHistory;

using MediatR;
using Planty.Contracts.Plants;
using Planty.Domain.Repositories;

public class GetPlantCareHistoryQueryHandler : IRequestHandler<GetPlantCareHistoryQuery, IEnumerable<CareEventResponse>>
{
    private readonly IPlantRepository _plantRepository;
    private readonly IWateringRepository _wateringRepository;
    private readonly IFertilizationRepository _fertilizationRepository;
    private readonly IPlantPictureRepository _pictureRepository;

    public GetPlantCareHistoryQueryHandler(
        IPlantRepository plantRepository, 
        IWateringRepository wateringRepository,
        IFertilizationRepository fertilizationRepository,
        IPlantPictureRepository pictureRepository)
    {
        _plantRepository = plantRepository;
        _wateringRepository = wateringRepository;
        _fertilizationRepository = fertilizationRepository;
        _pictureRepository = pictureRepository;
    }

    public async Task<IEnumerable<CareEventResponse>> Handle(GetPlantCareHistoryQuery request, CancellationToken cancellationToken)
    {
        // First verify the plant exists and belongs to the user
        var plant = await _plantRepository.GetByIdAsync(request.PlantId, cancellationToken);
        if (plant == null || plant.UserId != request.UserId)
            return Enumerable.Empty<CareEventResponse>();

        // Get both watering and fertilization events
        var waterings = await _wateringRepository.GetByPlantIdAsync(request.PlantId, cancellationToken);
        var fertilizations = await _fertilizationRepository.GetByPlantIdAsync(request.PlantId, cancellationToken);
        var pictures = await _pictureRepository.GetByPlantIdAsync(request.PlantId, cancellationToken);

        // Convert to care events
        var wateringEvents = waterings.Select(w => new CareEventResponse(
            w.Id,
            "Watering",
            w.WateredAt,
            w.Notes,
            null
        ));

        var fertilizationEvents = fertilizations.Select(f => new CareEventResponse(
            f.Id,
            "Fertilization",
            f.FertilizedAt,
            f.Notes,
            null
        ));

        var pictureEvents = pictures.Select(p => new CareEventResponse(
            p.Id,
            "Picture",
            p.TakenAt,
            p.Notes,
            $"/api/plants/pictures/{p.Id}"
        ));

        // Combine and sort by timestamp descending (most recent first)
        return wateringEvents
            .Concat(fertilizationEvents)
            .Concat(pictureEvents)
            .OrderByDescending(e => e.Timestamp)
            .ToList();
    }
}
