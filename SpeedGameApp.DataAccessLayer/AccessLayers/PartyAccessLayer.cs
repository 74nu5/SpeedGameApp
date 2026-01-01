namespace SpeedGameApp.DataAccessLayer.AccessLayers;

using Microsoft.EntityFrameworkCore;

using SpeedGameApp.DataAccessLayer.Entities;

public sealed class PartyAccessLayer(AppContext context)
{
    public async Task<Party> CreatePartyAsync(string partyName, CancellationToken cancellationToken)
    {
        var newParty = new Party { Name = partyName };
        _ = context.Parties.Add(newParty);
        _ = await context.SaveChangesAsync(cancellationToken);
        return newParty;
    }

    public async Task<Team?> CreateTeamPartyAsync(Guid partyId, string? teamName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(teamName))
            return default;

        var party = context.Parties.FirstOrDefault(p => p.Id == partyId);

        if (party == null)
            return default;

        var newTeam = new Team { Name = teamName };
        party.Teams.Add(newTeam);
        _ = await context.SaveChangesAsync(cancellationToken);
        return newTeam;
    }

    public async Task<Party?> GetPartyAsync(Guid id, CancellationToken cancellationToken)
        => await context.Parties.Include(p => p.Teams).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IEnumerable<Party>> GetPartiesAsync(CancellationToken cancellationToken) => await context.Parties.Include(p => p.Teams).ToListAsync(cancellationToken);

    public async Task DeletePartyAsync(Guid id, CancellationToken cancellationToken) => await context.Parties.Where(party => party.Id == id).ExecuteDeleteAsync(cancellationToken);

    public async Task UpdateScoreAsync(Guid key, int teamScore, CancellationToken cancellationToken)
    {
        var dbTeam = await context.Teams.FirstOrDefaultAsync(t => t.Id == key, cancellationToken);

        if (dbTeam is null)
            return;

        dbTeam.Score = teamScore;
        _ = await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTeamAsync(Guid partyId, Guid teamId, CancellationToken cancellationToken)
    {
        var party = await context.Parties.FirstOrDefaultAsync(p => p.Id == partyId, cancellationToken: cancellationToken);
        if (party is null) return;

        _ = await context.Teams.Where(t => t.Id == teamId).ExecuteDeleteAsync(cancellationToken);
    }
}
