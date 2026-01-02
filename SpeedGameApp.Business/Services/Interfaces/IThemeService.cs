namespace SpeedGameApp.Business.Services.Interfaces;

using SpeedGameApp.Business.Data;

/// <summary>
///     Service responsible for theme management (loading, selection, generation, visibility).
/// </summary>
public interface IThemeService
{
    /// <summary>
    ///     Gets all themes for a party (loads from database if needed).
    /// </summary>
    Task<IEnumerable<ThemeDto>> GetThemesAsync(Guid partyId);

    /// <summary>
    ///     Selects a theme for a team.
    /// </summary>
    void SelectTheme(Guid partyId, Guid? teamId, ThemeDto theme);

    /// <summary>
    ///     Confirms the choice of a theme.
    /// </summary>
    void ChoiceTheme(Guid partyId, Guid? teamId, ThemeDto theme);

    /// <summary>
    ///     Resets all theme choices for a party.
    /// </summary>
    void ResetThemesChoices(Guid partyId);

    /// <summary>
    ///     Hides themes display for a party.
    /// </summary>
    void HideTheme(Guid partyId);

    /// <summary>
    ///     Shows themes display for a party.
    /// </summary>
    void ShowTheme(Guid partyId);

    /// <summary>
    ///     Generates randomized themes for a party.
    /// </summary>
    void GenerateThemes(Guid partyId);
}
