namespace SpeedGameApp.Pages.Game;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Shared.Components.Responses;

/// <summary>
///     The party team play.
/// </summary>
public sealed partial class PartyTeamPlay : PartyPageBase
{
    private Proposition? proposition;

    private QCM? qcm;

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        this.CurrentParty.PartyReset += this.CurrentPartyOnPartyResetAsync;
    }

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

    private async void CurrentPartyOnPartyResetAsync(object? sender, EventArgs e)
    {
        if (this.proposition is not null)
            await this.proposition.ResetAsync().ConfigureAwait(true);

        if (this.qcm is not null)
            await this.qcm.ResetAsync().ConfigureAwait(true);
    }
}
