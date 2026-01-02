namespace SpeedGameApp.Business.Validators;

using FluentValidation;

/// <summary>
///     Validator for team names.
/// </summary>
public sealed class TeamNameValidator : AbstractValidator<string>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TeamNameValidator"/> class.
    /// </summary>
    public TeamNameValidator()
    {
        RuleFor(name => name)
            .NotEmpty().WithMessage("Le nom de l'équipe est requis.")
            .MinimumLength(2).WithMessage("Le nom de l'équipe doit contenir au moins 2 caractères.")
            .MaximumLength(30).WithMessage("Le nom de l'équipe ne peut pas dépasser 30 caractères.");
    }
}
