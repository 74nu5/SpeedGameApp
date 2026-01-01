namespace SpeedGameApp.Business.Services.Implementations;

using System.Collections.Concurrent;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services.Interfaces;
using SpeedGameApp.DataAccessLayer.Entities;

/// <summary>
///     In-memory repository for party storage.
/// </summary>
internal sealed class PartyRepository : IPartyRepository
{
    private readonly IDictionary<Guid, PartyDto> parties = new ConcurrentDictionary<Guid, PartyDto>();
    private readonly IPartyEventPublisher eventPublisher;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PartyRepository"/> class.
    /// </summary>
    /// <param name="eventPublisher">The event publisher.</param>
    public PartyRepository(IPartyEventPublisher eventPublisher)
        => this.eventPublisher = eventPublisher;

    /// <inheritdoc/>
    public IReadOnlyDictionary<Guid, PartyDto> Parties
        => this.parties.AsReadOnly();

    /// <inheritdoc/>
    public PartyDto LoadParty(PartyDto partyDto)
    {
        this.parties.Add(partyDto.Id, partyDto);
        return partyDto;
    }

    /// <inheritdoc/>
    public void AddParty(Guid partyId, string partyName)
    {
        var party = new PartyDto(partyId, partyName);
        this.parties.Add(partyId, party);
        this.eventPublisher.OnPartyChanged(party);
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public void RemoveTeam(Guid partyId, Guid teamId)
    {
        _ = this.parties[partyId].Teams.Remove(teamId);
        this.parties[partyId].OnPartyChanged();
    }

    /// <inheritdoc/>
    public void DeleteParty(Guid partyId)
        => this.parties.Remove(partyId);

    /// <inheritdoc/>
    public void DeleteAllParties()
        => this.parties.Clear();

    /// <inheritdoc/>
    public bool ExistsParty(Guid partyId)
        => this.parties.ContainsKey(partyId);

    /// <inheritdoc/>
    public PartyDto? GetParty(Guid partyId)
        => this.parties.TryGetValue(partyId, out var party) ? party : null;
}
