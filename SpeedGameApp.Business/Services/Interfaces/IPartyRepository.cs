namespace SpeedGameApp.Business.Services.Interfaces;

using SpeedGameApp.Business.Data;
using SpeedGameApp.DataAccessLayer.Entities;

/// <summary>
///     Repository interface for party storage and retrieval.
/// </summary>
public interface IPartyRepository
{
    /// <summary>
    ///     Gets all parties.
    /// </summary>
    IReadOnlyDictionary<Guid, PartyDto> Parties { get; }

    /// <summary>
    ///     Loads a party into the repository.
    /// </summary>
    /// <param name="partyDto">The party to load.</param>
    /// <returns>The loaded party.</returns>
    PartyDto LoadParty(PartyDto partyDto);

    /// <summary>
    ///     Adds a new party.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <param name="partyName">The party name.</param>
    void AddParty(Guid partyId, string partyName);

    /// <summary>
    ///     Adds a team to a party.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <param name="team">The team to add.</param>
    /// <returns>The team id if successful, null otherwise.</returns>
    Guid? AddTeamParty(Guid partyId, Team team);

    /// <summary>
    ///     Removes a team from a party.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <param name="teamId">The team id.</param>
    void RemoveTeam(Guid partyId, Guid teamId);

    /// <summary>
    ///     Deletes a party.
    /// </summary>
    /// <param name="partyId">The party id to delete.</param>
    void DeleteParty(Guid partyId);

    /// <summary>
    ///     Deletes all parties.
    /// </summary>
    void DeleteAllParties();

    /// <summary>
    ///     Checks if a party exists.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <returns>True if the party exists, false otherwise.</returns>
    bool ExistsParty(Guid partyId);

    /// <summary>
    ///     Gets a party by id.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <returns>The party if found, null otherwise.</returns>
    PartyDto? GetParty(Guid partyId);
}
