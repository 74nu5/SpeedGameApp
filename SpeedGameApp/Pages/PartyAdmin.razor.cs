namespace SpeedGameApp.Pages;

using Microsoft.AspNetCore.Components;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services.Interfaces;
using SpeedGameApp.DataEnum;

public sealed partial class PartyAdmin : PartyPageBase
{
    private bool showDeleteTeamDialog = false;
    private string deleteTeamDialogMessage = string.Empty;
    private Guid? teamToDeleteId = null;

    /// <summary>
    ///     Changes the theme of the current party.
    /// </summary>
    /// <param name="theme">The new theme to apply.</param>
    private void ChangeTheme(PartyTheme theme)
    {
        if (this.CurrentParty != null)
        {
            this.CurrentParty.Theme = theme;
            this.CurrentParty.OnPartyChanged();
            this.StateHasChanged();
        }
    }

    /// <summary>
    ///     Gets the CSS class for a response type button based on whether it's selected.
    /// </summary>
    /// <param name="responseType">The response type to check.</param>
    /// <returns>The CSS classes to apply.</returns>
    private string GetResponseTypeButtonClass(ResponseType responseType)
    {
        var isSelected = this.CurrentParty.CurrentResponseType == responseType;
        var baseClasses = "px-4 py-2 rounded-lg transition-all font-semibold inline-flex items-center";

        if (isSelected)
        {
            return $"{baseClasses} bg-3b-blue text-white border-2 border-3b-yellow shadow-medium";
        }

        return $"{baseClasses} bg-gray-200 text-gray-700 hover:bg-gray-300 border-2 border-transparent";
    }

    /// <inheritdoc />
    public PartyAdmin(IPartyManagementService partyManagementService, IQcmService qcmService, IGameplayService gameplayService, IThemeService themeService, NavigationManager navigationManager)
            : base(partyManagementService, qcmService, gameplayService, themeService, navigationManager)
    {
    }

    private async Task AddPointsAsync(TeamDto teamDto, int points)
    {
        this.CancellationTokenSource = new();
        await this.GameplayService.AddPointsAsync(teamDto, points, this.CancellationTokenSource.Token);
    }

    private void SetResponse(ResponseType responseType)
        => this.GameplayService.SetCurrentResponse(this.PartyId, responseType);

    private void ShowDeleteTeamDialog(Guid teamId, string teamName)
    {
        this.teamToDeleteId = teamId;
        this.deleteTeamDialogMessage = $"Voulez-vous vraiment supprimer l'équipe '{teamName}' ? Cette action est irréversible.";
        this.showDeleteTeamDialog = true;
    }

    private async Task ConfirmDeleteTeam()
    {
        if (this.teamToDeleteId.HasValue)
        {
            await this.DeleteTeamAsync(this.PartyId, this.teamToDeleteId.Value);
            this.teamToDeleteId = null;
        }

        this.showDeleteTeamDialog = false;
    }

    private void CancelDeleteTeam()
    {
        this.teamToDeleteId = null;
        this.showDeleteTeamDialog = false;
    }

    private async Task DeleteTeamAsync(Guid partyId, Guid teamId)
    {
        this.CancellationTokenSource = new();
        await this.PartyManagementService.DeleteTeamAsync(partyId, teamId, this.CancellationTokenSource.Token);
        await this.InvokeAsync(this.StateHasChanged);
    }

    private void RandomizeQcm()
        => this.QcmService.SetRandomQcm(this.PartyId);

    private void ResetQuestion()
        => this.GameplayService.ResetTeam(this.PartyId);

    private void HideTheme()
        => this.ThemeService.HideTheme(this.PartyId);

    private void ShowTheme()
    {
        this.ThemeService.GenerateThemes(this.PartyId);
        this.ThemeService.ShowTheme(this.PartyId);
    }

    private string GetCardCss(ThemeDto themeDto)
        => (this.TeamId, themeDto.AlreadyTaken, themeDto.Team?.Id) switch
        {
            (null, _, _) => "card bg-light",
            (_, true, var teamId) when teamId == this.TeamId => "card bg-success",
            (_, true, { } teamId) when this.CurrentParty.Teams.ContainsKey(teamId) => "card bg-danger",
            (_, true, null) => "card bg-secondary",
            _ => "card bg-light",
        };
}
