namespace Planty.Application.Tests.Commands;

using FluentAssertions;
using FluentValidation.TestHelper;
using Planty.Application.Commands.CreatePlant;

public class CreatePlantCommandValidatorTests
{
    private readonly CreatePlantCommandValidator _validator;

    public CreatePlantCommandValidatorTests()
    {
        _validator = new CreatePlantCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new CreatePlantCommand(string.Empty, "Species", null, 7, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Species_Is_Empty()
    {
        var command = new CreatePlantCommand("Name", string.Empty, null, 7, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Species);
    }

    [Fact]
    public void Should_Have_Error_When_WateringIntervalDays_Is_Zero()
    {
        var command = new CreatePlantCommand("Name", "Species", null, 0, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.WateringIntervalDays);
    }

    [Fact]
    public void Should_Have_Error_When_WateringIntervalDays_Exceeds_365()
    {
        var command = new CreatePlantCommand("Name", "Species", null, 366, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.WateringIntervalDays);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid_Command()
    {
        var command = new CreatePlantCommand("Valid Name", "Valid Species", "Valid Description", 7, "Valid Location");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
