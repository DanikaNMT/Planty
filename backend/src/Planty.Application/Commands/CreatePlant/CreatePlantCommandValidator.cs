namespace PlantApp.Application.Commands.CreatePlant;

using FluentValidation;

public class CreatePlantCommandValidator : AbstractValidator<CreatePlantCommand>
{
    public CreatePlantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Plant name is required and must not exceed 100 characters");

        RuleFor(x => x.Species)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Plant species is required and must not exceed 100 characters");

        RuleFor(x => x.WateringIntervalDays)
            .GreaterThan(0)
            .LessThanOrEqualTo(365)
            .WithMessage("Watering interval must be between 1 and 365 days");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.Location)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Location))
            .WithMessage("Location must not exceed 100 characters");
    }
}