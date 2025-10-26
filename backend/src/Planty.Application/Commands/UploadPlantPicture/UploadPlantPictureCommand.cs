namespace Planty.Application.Commands.UploadPlantPicture;

using MediatR;
using Planty.Contracts.Plants;

public record UploadPlantPictureCommand(
    Guid PlantId,
    Guid UserId,
    Stream FileStream,
    string FileName,
    string? Notes
) : IRequest<PlantPictureResponse>;
