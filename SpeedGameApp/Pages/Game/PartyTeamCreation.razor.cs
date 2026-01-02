namespace SpeedGameApp.Pages.Game;

public sealed partial class PartyTeamCreation : PartyPageBase
{
    private string? teamName = string.Empty;

    private async Task CreateTeamPartyAsync()
    {
        if (string.IsNullOrWhiteSpace(this.teamName))
            return;

        var cancellationTokenSource = new CancellationTokenSource();

        var result = await this.PartyManagementService.CreateTeamPartyAsync(this.PartyId, this.teamName, cancellationTokenSource.Token);

        if (result.IsSuccess)
            this.NavigationManager.NavigateTo($"/party/{this.PartyId}/team/{result.Value}/play");
        // TODO: Handle error case - could display error message to user
    }
}
