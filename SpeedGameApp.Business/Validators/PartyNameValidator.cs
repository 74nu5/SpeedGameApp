namespace SpeedGameApp.Business.Validators;

using FluentValidation;

/// <summary>
///     Validator for party names.
/// </summary>
public sealed class PartyNameValidator : AbstractValidator<string>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PartyNameValidator" /> class.
    /// </summary>
    public PartyNameValidator()
        => this.RuleFor(static name => name)
               .NotEmpty().WithMessage("Le nom de la partie est requis.")
               .MinimumLength(3).WithMessage("Le nom de la partie doit contenir au moins 3 caractères.")
               .MaximumLength(50).WithMessage("Le nom de la partie ne peut pas dépasser 50 caractères.");
}
