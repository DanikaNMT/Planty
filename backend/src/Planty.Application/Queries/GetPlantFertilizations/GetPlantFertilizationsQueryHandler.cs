namespace Planty.Application.Queries.GetPlantFertilizations;

using MediatR;
using Planty.Contracts.Plants;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

public class GetPlantFertilizationsQueryHandler : IRequestHandler<GetPlantFertilizationsQuery, IEnumerable<FertilizationResponse>>
{
    private readonly IPlantRepository _plantRepository;
    private readonly IFertilizationRepository _fertilizationRepository;

    public GetPlantFertilizationsQueryHandler(IPlantRepository plantRepository, IFertilizationRepository fertilizationRepository)
    {
        _plantRepository = plantRepository;
        _fertilizationRepository = fertilizationRepository;
    }

    public async Task<IEnumerable<FertilizationResponse>> Handle(GetPlantFertilizationsQuery request, CancellationToken cancellationToken)
    {
        // First verify the plant exists and belongs to the user
        var plant = await _plantRepository.GetByIdAsync(request.PlantId, cancellationToken);
        if (plant == null || plant.UserId != request.UserId)
            return Enumerable.Empty<FertilizationResponse>();

        var fertilizations = await _fertilizationRepository.GetByPlantIdAsync(request.PlantId, cancellationToken);
        return fertilizations.Select(MapToResponse);
    }

    private static FertilizationResponse MapToResponse(Fertilization fertilization)
    {
        return new FertilizationResponse(
            fertilization.Id,
            fertilization.FertilizedAt,
            fertilization.Notes,
            fertilization.UserId,
            fertilization.User?.UserName ?? "Unknown"
        );
    }
}
