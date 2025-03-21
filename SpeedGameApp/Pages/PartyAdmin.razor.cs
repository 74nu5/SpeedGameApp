namespace SpeedGameApp.Pages;

using SpeedGameApp.Business.Data;
using SpeedGameApp.DataEnum;
using SpeedGameApp.Shared.Components;

public sealed partial class PartyAdmin : PartyPageBase
{
    private TimeOnly propositionDuration = TimeOnly.MinValue;

    private Timer? timerComponent;

    private TimeSpan? time;
    private bool resetEnabled = true;

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync().ConfigureAwait(true);
        this.CurrentParty.TickTimer += this.CurrentPartyOnTickTimerAsync;
        this.CurrentParty.ResponseStarted += this.CurrentPartyOnResponseStartedAsync;
        this.CurrentParty.TimerEnd += this.CurrentPartyOnTimerEndAsync;
    }

    private void CurrentPartyOnTimerEndAsync(object? sender, EventArgs e)
    {
        this.resetEnabled = true;
    }

    private void CurrentPartyOnResponseStartedAsync(object? sender, EventArgs e)
    {
        // Set the initial time
        this.time = this.CurrentParty.ElapsedTime;
        this.resetEnabled = false;
    }


    private void CurrentPartyOnTickTimerAsync(object? sender, bool e)
        // Subscribe to the timer
        => this.timerComponent?.AdvanceTime();

    private async Task AddPointsAsync(TeamDto teamDto, int points)
    {
        this.CancellationTokenSource = new();
        await this.GameService.AddPointsAsync(teamDto, points, this.CancellationTokenSource.Token);
    }

    private void SetResponse(ResponseType responseType)
        => this.GameService.SetCurrentResponse(this.PartyId, responseType);

    private async Task DeleteTeamAsync(Guid partyId, Guid teamId)
    {
        this.CancellationTokenSource = new();
        await this.GameService.DeleteTeamAsync(partyId, teamId, this.CancellationTokenSource.Token);
        await this.InvokeAsync(this.StateHasChanged);
    }

    private void RandomizeQcm()
        => this.GameService.SetRandomQcm(this.PartyId);

    private void ResetQuestion()
        => this.GameService.ResetTeam(this.PartyId);

    private void HideTheme()
        => this.GameService.HideTheme(this.PartyId);

    private void ShowTheme()
    {
        this.GameService.GenerateThemes(this.PartyId);
        this.GameService.ShowTheme(this.PartyId);
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

    private void SetTimedPropositionResponse()
        => this.GameService.SetCurrentTimedPropositionResponse(this.PartyId, this.propositionDuration);

    private void ResumeTimedPropositionResponse()
        => this.GameService.ResumeTimedPropositionResponse(this.PartyId);
}
