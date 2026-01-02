namespace SpeedGameApp.Business.Services.Implementations;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services.Interfaces;
using SpeedGameApp.Business.Services.Models;
using SpeedGameApp.DataEnum;

/// <summary>
///     Manages the game state of parties (buzz, responses, scores, etc.).
/// </summary>
internal sealed class PartyStateManager(IPartyRepository repository) : IPartyStateManager
{
    private readonly IPartyRepository repository = repository;

    /// <inheritdoc/>
    public void AddPoints(TeamDto teamDto, int points)
    {
        var party = this.repository.Parties.FirstOrDefault(kvp => kvp.Value.Teams.Any(t => t.Key == teamDto.Id)).Value;
        teamDto.AddPoint(points);
        party.OnPartyChanged();
    }

    /// <inheritdoc/>
    public void SetCurrentResponse(Guid partyId, ResponseType responseType)
    {
        var party = this.repository.GetParty(partyId);
        if (party is null)
            return;

        party.CurrentResponseType = responseType;
        party.OnPartyChanged();
    }

    /// <inheritdoc/>
    public void BuzzTeam(Guid partyId, Guid teamId)
    {
        var party = this.repository.GetParty(partyId);
        if (party is null)
            return;

        var (_, team) = party.Teams.FirstOrDefault(pair => pair.Key == teamId);

        if (!party.AlreadyResponse)
        {
            team.Buzz = true;
            party.AlreadyResponse = true;
        }

        party.OnPartyChanged();
    }

    /// <inheritdoc/>
    public void ResetTeam(Guid partyId)
    {
        var party = this.repository.GetParty(partyId);
        if (party is null)
            return;

        party.AlreadyResponse = false;

        foreach (var (_, team) in party.Teams)
        {
            team.Buzz = false;
            team.Response = string.Empty;
        }

        party.OnPartyChanged();
        party.OnPartyReset();
    }

    /// <inheritdoc/>
    public void PropositionTeam(Guid partyId, Guid teamId, string response)
    {
        var party = this.repository.GetParty(partyId);
        if (party is null)
            return;

        var (_, team) = party.Teams.FirstOrDefault(pair => pair.Key == teamId);

        if (!party.AlreadyResponse)
        {
            team.Response = response;
            party.AlreadyResponse = true;
        }

        party.OnPartyChanged();
    }

    /// <inheritdoc/>
    public void PropositionQcmTeam(Guid partyId, Guid teamId, string response)
    {
        var party = this.repository.GetParty(partyId);
        if (party is null)
            return;

        var (_, team) = party.Teams.FirstOrDefault(pair => pair.Key == teamId);

        if (!team.AlreadyQcmResponse)
        {
            team.Response = response;
            team.QcmValidResponse = team.Response == party.CurrentQcm?.Response;
        }

        party.OnPartyChanged();
    }

    /// <inheritdoc/>
    public void SetCurrentQcm(Guid partyId, QcmQuestionDto question)
    {
        var party = this.repository.GetParty(partyId);
        if (party is null)
            return;

        party.CurrentQcm = question;
        party.OnPartyChanged();
    }
}
