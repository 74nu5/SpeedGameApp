namespace SpeedGameApp.Pages.Game;

using Microsoft.AspNetCore.Components;

using SpeedGameApp.Business.Services.Interfaces;

public sealed partial class PartyTeamCreation : PartyPageBase
{
    private string? teamName = string.Empty;
    private string errorMessage = string.Empty;

    /// <inheritdoc />
    public PartyTeamCreation(IPartyManagementService partyManagementService, IQcmService qcmService, IGameplayService gameplayService, IThemeService themeService, NavigationManager navigationManager)
            : base(partyManagementService, qcmService, gameplayService, themeService, navigationManager)
    {
    }

    private async Task CreateTeamPartyAsync()
    {
        if (string.IsNullOrWhiteSpace(this.teamName))
        {
            this.errorMessage = "Le nom de l'équipe ne peut pas être vide.";
            await this.InvokeAsync(this.StateHasChanged);
            return;
        }

        var cancellationTokenSource = new CancellationTokenSource();

        var result = await this.PartyManagementService.CreateTeamPartyAsync(this.PartyId, this.teamName, cancellationTokenSource.Token);

        if (result.IsSuccess)
        {
            this.NavigationManager.NavigateTo($"/party/{this.PartyId}/team/{result.Value}/play");
        }
        else
        {
            this.errorMessage = result.Error;
            await this.InvokeAsync(this.StateHasChanged);
        }
    }

    private void ClearError()
    {
        this.errorMessage = string.Empty;
    }
}
