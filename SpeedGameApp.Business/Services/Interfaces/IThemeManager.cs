namespace SpeedGameApp.Business.Services.Interfaces;

using SpeedGameApp.Business.Data;

/// <summary>
///     Interface for managing party themes.
/// </summary>
public interface IThemeManager
{
    /// <summary>
    ///     Loads themes for a party.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <param name="themes">The themes to load.</param>
    void LoadThemes(Guid partyId, IEnumerable<ThemeDto> themes);

    /// <summary>
    ///     Marks a theme as selected in the random theme list.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <param name="teamId">The team id.</param>
    /// <param name="theme">The theme to select.</param>
    void SelectTheme(Guid partyId, Guid? teamId, ThemeDto theme);

    /// <summary>
    ///     Assigns a theme to a team.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <param name="teamId">The team id.</param>
    /// <param name="theme">The theme to assign.</param>
    void ChoiceTheme(Guid partyId, Guid? teamId, ThemeDto theme);

    /// <summary>
    ///     Resets all theme choices for a party.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    void ResetThemesChoices(Guid partyId);
}
