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
        var command = new CreatePlantCommand(string.Empty, null, null, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Description_Is_Null()
    {
        var command = new CreatePlantCommand("Name", null, null, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Exceeds_MaxLength()
    {
        var command = new CreatePlantCommand("Name", null, new string('a', 501), null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid_Command()
    {
        var command = new CreatePlantCommand("Valid Name", Guid.NewGuid(), "Valid Description", null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Only_Name_Provided()
    {
        var command = new CreatePlantCommand("Valid Name", null, null, null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
