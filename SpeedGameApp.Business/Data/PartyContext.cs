namespace SpeedGameApp.Business.Data;

using System.Collections.Concurrent;

using SpeedGameApp.Business.Services.Models;
using SpeedGameApp.DataAccessLayer.Entities;
using SpeedGameApp.DataEnum;

internal sealed class PartyContext
{
    private readonly IDictionary<Guid, PartyDto> parties = new ConcurrentDictionary<Guid, PartyDto>();

    public event EventHandler<PartyDto>? PartyChanged;

    public IReadOnlyDictionary<Guid, PartyDto> Parties
        => this.parties.AsReadOnly();

    public void DeleteParty(Guid id) => this.parties.Remove(id);

    public bool ExistsParty(Guid id) => this.Parties.ContainsKey(id);

    public void AddPoints(TeamDto teamDto, int points)
    {
        var party = this.parties.FirstOrDefault(kvp => kvp.Value.Teams.Any(t => t.Key == teamDto.Id)).Value;
        teamDto.AddPoint(points);
        party.OnPartyChanged();
    }

    public void DeleteAllParties() => this.parties.Clear();

    public void SetCurrentResponse(Guid partyId, ResponseType responseType)
    {
        this.parties[partyId].CurrentResponseType = responseType;
        this.parties[partyId].OnPartyChanged();
    }

    public void BuzzTeam(Guid partyId, Guid teamId)
    {
        var (_, team) = this.parties[partyId].Teams.FirstOrDefault(pair => pair.Key == teamId);

        if (!this.parties[partyId].AlreadyResponse)
        {
            team.Buzz = true;
            this.parties[partyId].AlreadyResponse = true;
        }

        this.parties[partyId].OnPartyChanged();
    }

    public void ResetTeam(Guid partyId)
    {
        this.parties[partyId].AlreadyResponse = false;

        foreach (var (_, team) in this.parties[partyId].Teams)
        {
            team.Buzz = false;
            team.Response = string.Empty;
        }

        this.parties[partyId].OnPartyChanged();
    }

    public void PropositionTeam(Guid partyId, Guid teamId, string response)
    {
        var (_, team) = this.parties[partyId].Teams.FirstOrDefault(pair => pair.Key == teamId);

        if (!this.parties[partyId].AlreadyResponse)
        {
            team.Response = response;
            this.parties[partyId].AlreadyResponse = true;
        }

        this.parties[partyId].OnPartyChanged();
    }

    public PartyDto LoadParty(PartyDto partyDto)
    {
        this.parties.Add(partyDto.Id, partyDto);
        return partyDto;
    }

    public void RemoveTeam(Guid partyId, Guid teamId)
    {
        _ = this.parties[partyId].Teams.Remove(teamId);
        this.parties[partyId].OnPartyChanged();
    }

    public void SetCurrentQcm(Guid partyId, QcmQuestionDto question)
    {
        var party = this.Parties[partyId];
        party.CurrentQcm = question;
        party.OnPartyChanged();
    }

    public void PropositionQcmTeam(Guid partyId, Guid teamId, string response)
    {
        var (_, team) = this.parties[partyId].Teams.FirstOrDefault(pair => pair.Key == teamId);

        if (!team.AlreadyQcmResponse)
        {
            team.Response = response;
            team.AlreadyQcmResponse = true;
        }

        this.parties[partyId].OnPartyChanged();
    }

    public void AddParty(Guid guidParty, string partyName)
    {
        var party = new PartyDto(guidParty, partyName);
        this.parties.Add(guidParty, party);
        this.OnPartyChanged(party);
    }

    public Guid? AddTeamParty(Guid partyId, Team team)
    {
        if (string.IsNullOrWhiteSpace(team.Name))
            return default;

        if (!this.parties.TryGetValue(partyId, out var party))
            return default;

        var newTeam = new TeamDto(team.Id, team.Name);
        party.Teams.Add(newTeam.Id, newTeam);
        party.OnPartyChanged();
        return newTeam.Id;
    }

    public void OnPartyChanged(PartyDto e)
        => this.PartyChanged?.Invoke(this, e);
}
