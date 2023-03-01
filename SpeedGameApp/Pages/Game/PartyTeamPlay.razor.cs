namespace SpeedGameApp.Pages.Game;

using SpeedGameApp.Business.Data;

/// <summary>
/// The party team play.
/// </summary>
public sealed partial class PartyTeamPlay : PartyPageBase
{
    private void BuzzTeam()
        => this.GameService.BuzzTeam(this.PartyId, this.CurrentTeam.Id);

    private void Callback(string response)
        => this.GameService.PropositionTeam(this.PartyId, this.CurrentTeam.Id, response);

    private void CallbackQcm(string response)
        => this.GameService.PropositionQcmTeam(this.PartyId, this.CurrentTeam.Id, response);

    private string GetTeamCss(TeamDto team)
        => team.Buzz switch
    {
        true => "list-group-item-success",
        _ => team.Id == this.TeamId ? "fw-bold" : string.Empty,
    };
}
