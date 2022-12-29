namespace SpeedGameApp.Pages.Game;

public partial class PartyTeamPlay : PartyPageBase
{
    private void BuzzTeam()
        => this.GameService.BuzzTeam(this.PartyId, this.CurrentTeam.Id);

    private void Callback(string response)
        => this.GameService.PropositionTeam(this.PartyId, this.CurrentTeam.Id, response);

    private void CallbackQcm(string response)
        => this.GameService.PropositionQcmTeam(this.PartyId, this.CurrentTeam.Id, response);
}
