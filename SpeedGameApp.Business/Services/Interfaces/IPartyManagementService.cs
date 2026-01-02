namespace SpeedGameApp.Business.Services.Interfaces;

using SpeedGameApp.Business.Common;
using SpeedGameApp.Business.Services.Models;

/// <summary>
///     Service responsible for party and team lifecycle management (CRUD operations).
/// </summary>
public interface IPartyManagementService
{
    /// <summary>
    ///     Event fired when a party changes.
    /// </summary>
    event EventHandler<PartyDto>? PartyChanged;

    /// <summary>
    ///     Gets all parties currently in memory.
    /// </summary>
    IReadOnlyDictionary<Guid, PartyDto> Parties { get; }

    /// <summary>
    ///     Creates a new party with validation.
    /// </summary>
    Task<Result<Guid>> CreatePartyAsync(string partyName, CancellationToken cancellationToken);

    /// <summary>
    ///     Creates a new team in a party with validation.
    /// </summary>
    Task<Result<Guid>> CreateTeamPartyAsync(Guid partyId, string teamName, CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves a party by ID (from memory or database).
    /// </summary>
    Task<PartyDto?> GetPartyAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves all parties from the database.
    /// </summary>
    Task<Dictionary<Guid, PartyDto>> GetDbPartiesAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Loads a party from the database into memory.
    /// </summary>
    Task LoadPartyAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    ///     Saves party data (team scores) to the database.
    /// </summary>
    Task SavePartyAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    ///     Deletes a party from memory.
    /// </summary>
    void DeleteParty(Guid id);

    /// <summary>
    ///     Deletes all parties from memory.
    /// </summary>
    void DeleteAllParties();

    /// <summary>
    ///     Deletes a party from the database.
    /// </summary>
    Task DeleteDbPartyAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    ///     Deletes a team from a party (both database and memory).
    /// </summary>
    Task DeleteTeamAsync(Guid partyId, Guid teamId, CancellationToken cancellationToken);
}
