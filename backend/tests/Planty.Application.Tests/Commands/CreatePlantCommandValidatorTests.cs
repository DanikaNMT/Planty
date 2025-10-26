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
        var command = new CreatePlantCommand(string.Empty, null, null, null, null, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Species_Is_Null()
    {
        var command = new CreatePlantCommand("Name", null, null, null, null, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Species);
    }

    [Fact]
    public void Should_Have_Error_When_Species_Exceeds_MaxLength()
    {
        var command = new CreatePlantCommand("Name", new string('a', 101), null, null, null, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Species);
    }

    [Fact]
    public void Should_Have_Error_When_WateringIntervalDays_Is_Zero()
    {
        var command = new CreatePlantCommand("Name", null, null, 0, null, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.WateringIntervalDays);
    }

    [Fact]
    public void Should_Have_Error_When_WateringIntervalDays_Exceeds_365()
    {
        var command = new CreatePlantCommand("Name", null, null, 366, null, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.WateringIntervalDays);
    }

    [Fact]
    public void Should_Not_Have_Error_When_WateringIntervalDays_Is_Null()
    {
        var command = new CreatePlantCommand("Name", null, null, null, null, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.WateringIntervalDays);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid_Command()
    {
        var command = new CreatePlantCommand("Valid Name", "Valid Species", "Valid Description", 7, null, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Only_Name_Provided()
    {
        var command = new CreatePlantCommand("Valid Name", null, null, null, null, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
