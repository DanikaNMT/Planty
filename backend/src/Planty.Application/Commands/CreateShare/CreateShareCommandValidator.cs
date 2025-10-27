namespace Planty.Application.Commands.CreateShare;

using FluentValidation;
using Planty.Contracts.Shares;

public class CreateShareCommandValidator : AbstractValidator<CreateShareCommand>
{
    public CreateShareCommandValidator()
    {
        RuleFor(x => x.ShareType)
            .IsInEnum().WithMessage("Invalid share type");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Invalid role");

        RuleFor(x => x.SharedWithUserEmail)
            .NotEmpty().WithMessage("Shared with user email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.PlantId)
            .NotNull().When(x => x.ShareType == ShareTypeDto.Plant)
            .WithMessage("Plant ID is required when sharing a plant");

        RuleFor(x => x.LocationId)
            .NotNull().When(x => x.ShareType == ShareTypeDto.Location)
            .WithMessage("Location ID is required when sharing a location");
        
        RuleFor(x => x.PlantId)
            .Null().When(x => x.ShareType == ShareTypeDto.Collection)
            .WithMessage("Plant ID must be null when sharing a collection");
        
        RuleFor(x => x.LocationId)
            .Null().When(x => x.ShareType == ShareTypeDto.Collection)
            .WithMessage("Location ID must be null when sharing a collection");
    }
}
