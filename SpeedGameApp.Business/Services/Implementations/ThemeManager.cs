namespace SpeedGameApp.Business.Services.Implementations;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services.Interfaces;

/// <summary>
///     Manages themes for parties.
/// </summary>
internal sealed class ThemeManager(IPartyRepository repository) : IThemeManager
{
    /// <inheritdoc/>
    public void LoadThemes(Guid partyId, IEnumerable<ThemeDto> themes)
    {
        var party = repository.GetParty(partyId);
        if (party is null) return;

        party.LoadThemes(themes.ToList());
    }

    /// <inheritdoc/>
    public void SelectTheme(Guid partyId, Guid? teamId, ThemeDto theme)
    {
        var party = repository.GetParty(partyId);
        if (party is null) return;

        var themeDto = party.RandomThemes.FirstOrDefault(t => t.Id == theme.Id);

        if (themeDto is null)
            return;

        themeDto.AlreadyTaken = true;
        party.OnPartyChanged();
    }

    /// <inheritdoc/>
    public void ChoiceTheme(Guid partyId, Guid? teamId, ThemeDto theme)
    {
        var party = repository.GetParty(partyId);
        if (party is null || teamId is null) return;

        var themeDto = party.Themes.FirstOrDefault(t => t.Id == theme.Id);

        if (themeDto is null)
            return;

        themeDto.Team = party.Teams[(Guid)teamId];
        party.OnPartyChanged();
    }

    /// <inheritdoc/>
    public void ResetThemesChoices(Guid partyId)
    {
        var party = repository.GetParty(partyId);
        if (party is null) return;

        foreach (var theme in party.Themes)
        {
            theme.Team = null;
        }

        party.OnPartyChanged();
    }
}
