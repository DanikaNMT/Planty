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
    private readonly IShareRepository _shareRepository;

    public GetPlantCareHistoryQueryHandler(
        IPlantRepository plantRepository, 
        IWateringRepository wateringRepository,
        IFertilizationRepository fertilizationRepository,
        IPlantPictureRepository pictureRepository,
        IShareRepository shareRepository)
    {
        _plantRepository = plantRepository;
        _wateringRepository = wateringRepository;
        _fertilizationRepository = fertilizationRepository;
        _pictureRepository = pictureRepository;
        _shareRepository = shareRepository;
    }

    public async Task<IEnumerable<CareEventResponse>> Handle(GetPlantCareHistoryQuery request, CancellationToken cancellationToken)
    {
        // Verify the plant exists
        var plant = await _plantRepository.GetByIdAsync(request.PlantId, cancellationToken);
        if (plant == null)
            return Enumerable.Empty<CareEventResponse>();

        // Check if user owns the plant or has access to it via sharing
        bool hasAccess = plant.UserId == request.UserId;
        if (!hasAccess)
        {
            var role = await _shareRepository.GetUserRoleForPlantAsync(request.PlantId, request.UserId, cancellationToken);
            hasAccess = role.HasValue;
        }

        if (!hasAccess)
            return Enumerable.Empty<CareEventResponse>();

        // Get all care events
        var waterings = await _wateringRepository.GetByPlantIdAsync(request.PlantId, cancellationToken);
        var fertilizations = await _fertilizationRepository.GetByPlantIdAsync(request.PlantId, cancellationToken);
        var pictures = await _pictureRepository.GetByPlantIdAsync(request.PlantId, cancellationToken);

        // Convert to care events with user information
        var wateringEvents = waterings.Select(w => new CareEventResponse(
            w.Id,
            "Watering",
            w.WateredAt,
            w.Notes,
            null,
            w.UserId,
            w.User?.UserName ?? "Unknown"
        ));

        var fertilizationEvents = fertilizations.Select(f => new CareEventResponse(
            f.Id,
            "Fertilization",
            f.FertilizedAt,
            f.Notes,
            null,
            f.UserId,
            f.User?.UserName ?? "Unknown"
        ));

        var pictureEvents = pictures.Select(p => new CareEventResponse(
            p.Id,
            "Picture",
            p.TakenAt,
            p.Notes,
            $"/api/plants/pictures/{p.Id}",
            p.UserId,
            p.User?.UserName ?? "Unknown"
        ));

        // Combine and sort by timestamp descending (most recent first)
        return wateringEvents
            .Concat(fertilizationEvents)
            .Concat(pictureEvents)
            .OrderByDescending(e => e.Timestamp)
            .ToList();
    }
}
