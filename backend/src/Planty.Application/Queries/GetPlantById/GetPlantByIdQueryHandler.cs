namespace Planty.Application.Queries.GetPlantById;

using MediatR;
using Planty.Application.Common;
using Planty.Contracts.Plants;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

public class GetPlantByIdQueryHandler : IRequestHandler<GetPlantByIdQuery, PlantResponse?>
{
    private readonly IPlantRepository _plantRepository;

    public GetPlantByIdQueryHandler(IPlantRepository plantRepository)
    {
        _plantRepository = plantRepository;
    }

    public async Task<PlantResponse?> Handle(GetPlantByIdQuery request, CancellationToken cancellationToken)
    {
        var plant = await _plantRepository.GetByIdAsync(request.Id, cancellationToken);
        if (plant == null || plant.UserId != request.UserId)
            return null;
        return PlantMapper.MapToResponse(plant);
    }
}