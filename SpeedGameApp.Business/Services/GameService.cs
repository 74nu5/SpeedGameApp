namespace SpeedGameApp.Business.Services;

using Microsoft.Extensions.DependencyInjection;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services.Models;
using SpeedGameApp.DataAccessLayer.AccessLayers;
using SpeedGameApp.DataEnum;

public sealed class GameService
{
    private readonly PartyContext context;

    private readonly PartyAccessLayer partyAccessLayer;

    private readonly QuestionAccessLayer questionAccessLayer;

    public GameService(IServiceProvider serviceProvider, PartyAccessLayer partyAccessLayer, QuestionAccessLayer questionAccessLayer)
    {
        this.context = serviceProvider.GetRequiredService<PartyContext>();
        this.partyAccessLayer = partyAccessLayer;
        this.questionAccessLayer = questionAccessLayer;
    }

    public event EventHandler<PartyDto>? PartyChanged
    {
        add => this.context.PartyChanged += value;
        remove => this.context.PartyChanged -= value;
    }

    public IReadOnlyDictionary<Guid, PartyDto> Parties
        => this.context.Parties;

    public async Task<Guid> CreatePartyAsync(string partyName, CancellationToken cancellationToken)
    {
        var party = await this.partyAccessLayer.CreatePartyAsync(partyName, cancellationToken);
        this.context.AddParty(party.Id, partyName);
        return party.Id;
    }

    public async Task<Guid?> CreateTeamPartyAsync(Guid partyId, string? teamName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(teamName))
            return default;

        if (!this.context.ExistsParty(partyId))
            return default;

        var team = await this.partyAccessLayer.CreateTeamPartyAsync(partyId, teamName, cancellationToken);

        if (team is null)
            return default;

        _ = this.context.AddTeamParty(partyId, team);
        return team.Id;
    }

    public async Task SavePartyAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!this.context.Parties.TryGetValue(id, out var party))
            return;

        foreach (var (key, team) in party.Teams)
            await this.partyAccessLayer.UpdateScoreAsync(key, team.Score, cancellationToken);
    }

    public async Task<PartyDto?> GetPartyAsync(Guid id, CancellationToken cancellationToken)
    {
        if (this.context.Parties.TryGetValue(id, out var party))
            return party;

        var dbParty = await this.partyAccessLayer.GetPartyAsync(id, cancellationToken);

        return dbParty is null ? default : this.context.LoadParty(PartyDto.FromDbParty(dbParty));
    }

    public async Task<Dictionary<Guid, PartyDto>> GetDbPartiesAsync(CancellationToken cancellationToken)
    {
        var dbParties = await this.partyAccessLayer.GetPartiesAsync(cancellationToken);

        return PartyDto.FromDbParties(dbParties);
    }

    public async Task DeleteDbPartyAsync(Guid id, CancellationToken cancellationToken) => await this.partyAccessLayer.DeletePartyAsync(id, cancellationToken);

    public async Task LoadPartyAsync(Guid id, CancellationToken cancellationToken)
    {
        var partyFound = await this.partyAccessLayer.GetPartyAsync(id, cancellationToken);

        if (partyFound is null)
            return;

        _ = this.context.LoadParty(PartyDto.FromDbParty(partyFound));
    }

    public async Task DeleteTeamAsync(Guid partyId, Guid teamId, CancellationToken cancellationToken)
    {
        await this.partyAccessLayer.DeleteTeamAsync(partyId, teamId, cancellationToken);
        this.context.RemoveTeam(partyId, teamId);
    }

    public void SetRandomQcm(Guid partyId)
    {
        var question = this.questionAccessLayer.GetRandom();
        this.context.SetCurrentQcm(partyId, QcmQuestionDto.FromQcmQuestion(question));
    }

    public async Task AddPointsAsync(TeamDto teamDto, int points, CancellationToken cancellationToken)
    {
        this.context.AddPoints(teamDto, points);
        await this.SavePartyAsync(teamDto.PartyId, cancellationToken);
    }

    public void BuzzTeam(Guid partyId, Guid teamId)
        => this.context.BuzzTeam(partyId, teamId);

    public void DeleteAllParties()
        => this.context.DeleteAllParties();

    public void DeleteParty(Guid id)
        => this.context.DeleteParty(id);

    public void PropositionQcmTeam(Guid partyId, Guid teamId, string response)
        => this.context.PropositionQcmTeam(partyId, teamId, response);

    public void PropositionTeam(Guid partyId, Guid teamId, string response)
        => this.context.PropositionTeam(partyId, teamId, response);

    public void ResetTeam(Guid partyId)
        => this.context.ResetTeam(partyId);

    public void SetCurrentResponse(Guid partyId, ResponseType responseType)
        => this.context.SetCurrentResponse(partyId, responseType);
}
