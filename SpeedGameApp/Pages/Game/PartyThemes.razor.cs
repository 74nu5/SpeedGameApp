namespace SpeedGameApp.Pages.Game;

using SpeedGameApp.Business.Data;

public partial class PartyThemes : PartyPageBase
{
    private IEnumerable<ThemeDto> themes = new List<ThemeDto>();

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        this.themes = await this.GameService.GetThemesAsync(this.PartyId);
    }

    private async Task SelectThemeAsync(ThemeDto theme)
    {
        if (this.TeamId is null || theme.Team is not null)
            return;

        this.CancellationTokenSource = new();
        this.GameService.ChoiceTheme(this.PartyId, this.TeamId, theme);
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
    }

    private string GetCardCss(ThemeDto themeDto)
        => (this.TeamId, themeDto.AlreadyTaken, themeDto.Team?.Id) switch
        {
            (null, _, _) => "card bg-light",
            (_, _, { } teamId) when teamId == this.TeamId => "card bg-success",
            (_, _, not null) => "card text-white bg-dark",
            _ => "card bg-light",
        };

    private void ResetChoices()
    {
         this.GameService.ResetThemesChoices(this.PartyId);
    }
}
