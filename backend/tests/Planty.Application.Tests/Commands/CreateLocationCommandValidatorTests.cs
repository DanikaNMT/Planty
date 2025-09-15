namespace Planty.Application.Tests.Commands;

using FluentValidation.TestHelper;
using Planty.Application.Commands.CreateLocation;

public class CreateLocationCommandValidatorTests
{
    private readonly CreateLocationCommandValidator _validator;

    public CreateLocationCommandValidatorTests()
    {
        _validator = new CreateLocationCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new CreateLocationCommand(string.Empty, "Description", Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Exceeds_MaxLength()
    {
        var longName = new string('a', 101);
        var command = new CreateLocationCommand(longName, "Description", Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Exceeds_MaxLength()
    {
        var longDescription = new string('a', 501);
        var command = new CreateLocationCommand("Name", longDescription, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        var command = new CreateLocationCommand("Name", "Description", Guid.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var command = new CreateLocationCommand("Living Room", "Main living area", Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Description_Is_Null()
    {
        var command = new CreateLocationCommand("Living Room", null, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}