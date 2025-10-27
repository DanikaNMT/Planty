namespace Planty.Application.Commands.UpdateShare;

using FluentValidation;

public class UpdateShareCommandValidator : AbstractValidator<UpdateShareCommand>
{
    public UpdateShareCommandValidator()
    {
        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Invalid role");
    }
}
