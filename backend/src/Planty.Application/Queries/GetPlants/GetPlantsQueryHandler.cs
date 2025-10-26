namespace Planty.Application.Queries.GetPlants;

using MediatR;
using Planty.Application.Common;
using Planty.Contracts.Plants;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

public class GetPlantsQueryHandler : IRequestHandler<GetPlantsQuery, IEnumerable<PlantResponse>>
{
    private readonly IPlantRepository _plantRepository;

    public GetPlantsQueryHandler(IPlantRepository plantRepository)
    {
        _plantRepository = plantRepository;
    }

    public async Task<IEnumerable<PlantResponse>> Handle(GetPlantsQuery request, CancellationToken cancellationToken)
    {
        var plants = await _plantRepository.GetAllByUserAsync(request.UserId, cancellationToken);
        return plants.Select(PlantMapper.MapToResponse);
    }
}