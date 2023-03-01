namespace SpeedGameApp.Business.Services;

using Microsoft.Extensions.DependencyInjection;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services.Models;
using SpeedGameApp.DataAccessLayer.AccessLayers;
using SpeedGameApp.DataEnum;

/// <summary>
///     Class which defines all game mathods.
/// </summary>
public sealed class GameService
{
    private readonly PartyContext context;

    private readonly PartyAccessLayer partyAccessLayer;

    private readonly QuestionAccessLayer questionAccessLayer;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GameService" /> class.
    /// </summary>
    /// <param name="serviceProvider">The DI service provider.</param>
    /// <param name="partyAccessLayer">The party access layer.</param>
    /// <param name="questionAccessLayer">The question access layer.</param>
    public GameService(IServiceProvider serviceProvider, PartyAccessLayer partyAccessLayer, QuestionAccessLayer questionAccessLayer)
    {
        this.context = serviceProvider.GetRequiredService<PartyContext>();
        this.partyAccessLayer = partyAccessLayer;
        this.questionAccessLayer = questionAccessLayer;
    }

    /// <summary>
    ///     Event fired when party changed.
    /// </summary>
    public event EventHandler<PartyDto>? PartyChanged
    {
        add => this.context.PartyChanged += value;
        remove => this.context.PartyChanged -= value;
    }

    /// <summary>
    ///     Gets the parties.
    /// </summary>
    public IReadOnlyDictionary<Guid, PartyDto> Parties
        => this.context.Parties;

    /// <summary>
    ///     Asynchronously creates a new party with the specified name.
    /// </summary>
    /// <param name="partyName">The name of the party to create.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation. The result of the task is the ID of the newly created party.</returns>
    public async Task<Guid> CreatePartyAsync(string partyName, CancellationToken cancellationToken)
    {
        // Create the party in the database
        var party = await this.partyAccessLayer.CreatePartyAsync(partyName, cancellationToken);

        // Add the party to the context
        this.context.AddParty(party.Id, partyName);

        // Return the ID of the newly created party
        return party.Id;
    }

    /// <summary>
    ///     Asynchronously creates a new team party with the specified parameters.
    /// </summary>
    /// <param name="partyId">The ID of the party to which the team belongs.</param>
    /// <param name="teamName">The name of the team to be created.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The ID of the newly created team, or null if the operation was unsuccessful.</returns>
    public async Task<Guid?> CreateTeamPartyAsync(Guid partyId, string? teamName, CancellationToken cancellationToken)
    {
        // Return default value if team name is null or empty
        if (string.IsNullOrWhiteSpace(teamName))
            return default;

        // Return default value if party does not exist
        if (!this.context.ExistsParty(partyId))
            return default;

        // Create team party and return default value if operation is unsuccessful
        var team = await this.partyAccessLayer.CreateTeamPartyAsync(partyId, teamName, cancellationToken);

        if (team is null)
            return default;

        // Add team to context and return its ID
        _ = this.context.AddTeamParty(partyId, team);
        return team.Id;
    }

    /// <summary>
    ///     Asynchronously saves a party with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the party to save.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SavePartyAsync(Guid id, CancellationToken cancellationToken)
    {
        // Check if the party exists in the context
        if (!this.context.Parties.TryGetValue(id, out var party))
            return;

        // Loop through each team in the party and update their score
        foreach (var (key, team) in party.Teams)
            await this.partyAccessLayer.UpdateScoreAsync(key, team.Score, cancellationToken);
    }

    /// <summary>
    ///     Asynchronously retrieves a party with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the party to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation. The result of the task is the party with the specified ID, or null if the party was not found.</returns>
    public async Task<PartyDto?> GetPartyAsync(Guid id, CancellationToken cancellationToken)
    {
        // Check if the party exists in the context
        if (this.context.Parties.TryGetValue(id, out var party))
            return party;

        // If not, retrieve the party from the database
        var dbParty = await this.partyAccessLayer.GetPartyAsync(id, cancellationToken);

        // If the party was not found in the database, return null
        // Otherwise, load the party into the context and return it
        return dbParty is null ? default : this.context.LoadParty(PartyDto.FromDbParty(dbParty));
    }

    /// <summary>
    /// Asynchronously retrieves all parties from the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation. The result of the task is a dictionary mapping party IDs to party data.</returns>
    public async Task<Dictionary<Guid, PartyDto>> GetDbPartiesAsync(CancellationToken cancellationToken)
    {
        // Retrieve the parties from the database
        var dbParties = await this.partyAccessLayer.GetPartiesAsync(cancellationToken);

        // Convert the database parties to DTOs
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
