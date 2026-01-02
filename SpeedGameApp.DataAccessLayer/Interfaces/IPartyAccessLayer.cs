namespace SpeedGameApp.DataAccessLayer.Interfaces;

using SpeedGameApp.DataAccessLayer.Entities;

/// <summary>
///     Interface for party data access operations.
/// </summary>
public interface IPartyAccessLayer
{
    /// <summary>
    ///     Creates a new party asynchronously.
    /// </summary>
    /// <param name="partyName">The name of the party.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created party.</returns>
    Task<Party> CreatePartyAsync(string partyName, CancellationToken cancellationToken);

    /// <summary>
    ///     Creates a new team for a party asynchronously.
    /// </summary>
    /// <param name="partyId">The party ID.</param>
    /// <param name="teamName">The team name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created team, or null if the operation failed.</returns>
    Task<Team?> CreateTeamPartyAsync(Guid partyId, string? teamName, CancellationToken cancellationToken);

    /// <summary>
    ///     Gets a party by ID asynchronously.
    /// </summary>
    /// <param name="id">The party ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The party if found, null otherwise.</returns>
    Task<Party?> GetPartyAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    ///     Gets all parties asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all parties.</returns>
    Task<IEnumerable<Party>> GetPartiesAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Deletes a party asynchronously.
    /// </summary>
    /// <param name="id">The party ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeletePartyAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    ///     Updates a team's score asynchronously.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <param name="teamScore">The new score.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateScoreAsync(Guid teamId, int teamScore, CancellationToken cancellationToken);

    /// <summary>
    ///     Deletes a team asynchronously.
    /// </summary>
    /// <param name="partyId">The party ID.</param>
    /// <param name="teamId">The team ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteTeamAsync(Guid partyId, Guid teamId, CancellationToken cancellationToken);
}
