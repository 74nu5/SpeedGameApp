namespace SpeedGameApp.DataAccessLayer.AccessLayers;

using Microsoft.EntityFrameworkCore;

using SpeedGameApp.DataAccessLayer.Entities;
using SpeedGameApp.DataAccessLayer.Interfaces;

/// <summary>
///     Data access layer for party operations.
/// </summary>
public sealed class PartyAccessLayer(SpeedGameDbContext context) : IPartyAccessLayer
{
    private readonly SpeedGameDbContext context = context;

    /// <inheritdoc/>
    public async Task<Party> CreatePartyAsync(string partyName, CancellationToken cancellationToken)
    {
        var newParty = new Party { Name = partyName };
        _ = this.context.Parties.Add(newParty);
        _ = await this.context.SaveChangesAsync(cancellationToken);
        return newParty;
    }

    /// <inheritdoc/>
    public async Task<Team?> CreateTeamPartyAsync(Guid partyId, string? teamName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(teamName))
            return default;

        var party = await this.context.Parties.FirstOrDefaultAsync(p => p.Id == partyId, cancellationToken);

        if (party is null)
            return default;

        var newTeam = new Team { Name = teamName };
        party.Teams.Add(newTeam);
        _ = await this.context.SaveChangesAsync(cancellationToken);
        return newTeam;
    }

    /// <inheritdoc/>
    public async Task<Party?> GetPartyAsync(Guid id, CancellationToken cancellationToken)
        => await this.context.Parties.Include(p => p.Teams).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    /// <inheritdoc/>
    public async Task<IEnumerable<Party>> GetPartiesAsync(CancellationToken cancellationToken)
        => await this.context.Parties.Include(p => p.Teams).ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task DeletePartyAsync(Guid id, CancellationToken cancellationToken)
        => await this.context.Parties.Where(party => party.Id == id).ExecuteDeleteAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task UpdateScoreAsync(Guid teamId, int teamScore, CancellationToken cancellationToken)
    {
        var dbTeam = await this.context.Teams.FirstOrDefaultAsync(t => t.Id == teamId, cancellationToken);

        if (dbTeam is null)
            return;

        dbTeam.Score = teamScore;
        _ = await this.context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteTeamAsync(Guid partyId, Guid teamId, CancellationToken cancellationToken)
    {
        var party = await this.context.Parties.FirstOrDefaultAsync(p => p.Id == partyId, cancellationToken);
        if (party is null)
            return;

        _ = await this.context.Teams.Where(t => t.Id == teamId).ExecuteDeleteAsync(cancellationToken);
    }
}
