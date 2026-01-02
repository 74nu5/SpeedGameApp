namespace SpeedGameApp.Business.Services.Implementations;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services.Interfaces;
using SpeedGameApp.DataAccessLayer.Interfaces;

/// <summary>
///     Service responsible for theme management (loading, selection, generation, visibility).
/// </summary>
public sealed class ThemeService(
    IThemeAccessLayer themeAccessLayer,
    IThemeManager themeManager,
    IPartyRepository partyRepository) : IThemeService
{
    /// <inheritdoc />
    public async Task<IEnumerable<ThemeDto>> GetThemesAsync(Guid partyId)
    {
        if (partyRepository.Parties[partyId].Themes.Any())
            return partyRepository.Parties[partyId].Themes;

        var allThemes = await themeAccessLayer.GetAllThemesAsync();

        themeManager.LoadThemes(partyId,
                                allThemes.Select(t => new ThemeDto
                                {
                                    Id = t.Id,
                                    Name = t.Name,
                                }));

        return partyRepository.Parties[partyId].Themes;
    }

    /// <inheritdoc />
    public void SelectTheme(Guid partyId, Guid? teamId, ThemeDto theme)
        => themeManager.SelectTheme(partyId, teamId, theme);

    /// <inheritdoc />
    public void ChoiceTheme(Guid partyId, Guid? teamId, ThemeDto theme)
        => themeManager.ChoiceTheme(partyId, teamId, theme);

    /// <inheritdoc />
    public void ResetThemesChoices(Guid partyId)
        => themeManager.ResetThemesChoices(partyId);

    /// <inheritdoc />
    public void HideTheme(Guid partyId)
    {
        partyRepository.Parties[partyId].ShowThemes = false;
        partyRepository.Parties[partyId].OnPartyChanged();
    }

    /// <inheritdoc />
    public void ShowTheme(Guid partyId)
    {
        partyRepository.Parties[partyId].ShowThemes = true;
        partyRepository.Parties[partyId].OnPartyChanged();
    }

    /// <inheritdoc />
    public void GenerateThemes(Guid partyId)
    {
        var currentParty = partyRepository.Parties[partyId];
        var random = Random.Shared;
        List<ThemeDto> themes = [];

        foreach (var (_, team) in currentParty.Teams)
        {
            var themesTeam = currentParty.Themes.Where(th => th.Team?.Id == team.Id).ToList();

            foreach (var themeTeam in themesTeam)
            {
                for (var i = 0; i < 5; i++)
                {
                    themes.Add(new() { Id = Guid.NewGuid(), Name = themeTeam.Name, Team = team });
                }
            }
        }

        var selectedThemeNames = themes.DistinctBy(theme => theme.Name).Select(theme => theme.Name);
        var otherThemes = currentParty.Themes.Where(th => !selectedThemeNames.Contains(th.Name));

        List<ThemeDto> otherThemesLimited = [];
        foreach (var otherTheme in otherThemes)
        {
            for (var i = 0; i < 5; i++)
            {
                otherThemesLimited.Add(new() { Id = Guid.NewGuid(), Name = otherTheme.Name, Team = null });
            }
        }

        themes.AddRange(themes.Count < 50 ? otherThemesLimited.Take(50 - themes.Count) : otherThemesLimited);

        var randomThemes = themes.OrderBy(_ => random.Next());
        currentParty.LoadRandomThemes(randomThemes);
    }
}
