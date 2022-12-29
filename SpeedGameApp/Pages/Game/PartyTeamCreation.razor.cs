namespace SpeedGameApp.Pages.Game;

public sealed partial class PartyTeamCreation : PartyPageBase
{
    private string? teamName = string.Empty;

    private async Task CreateTeamPartyAsync()
    {
        var cancellationTokenSource = new CancellationTokenSource();

        var teamId = await this.GameService.CreateTeamPartyAsync(this.PartyId, this.teamName, cancellationTokenSource.Token);
        this.NavigationManager.NavigateTo($"/party/{this.PartyId}/team/{teamId}/play");
    }
}
