namespace SpeedGameApp.Business.Services;

using Microsoft.Extensions.DependencyInjection;
using SpeedGameApp.Business.Context;
using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services.Models;
using SpeedGameApp.DataAccessLayer.AccessLayers;
using SpeedGameApp.DataEnum;

/// <summary>
///     Class which defines all game mathods.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="GameService" /> class.
/// </remarks>
/// <param name="serviceProvider">The DI service provider.</param>
/// <param name="partyAccessLayer">The party access layer.</param>
/// <param name="questionAccessLayer">The question access layer.</param>
/// <param name="themeAccessLayer">The theme access layer.</param>
public sealed class GameService(
    IServiceProvider serviceProvider,
    PartyAccessLayer partyAccessLayer,
    QuestionAccessLayer questionAccessLayer,
    ThemeAccessLayer themeAccessLayer)
{
    private readonly PartiesContext context = serviceProvider.GetRequiredService<PartiesContext>();

    private readonly PartyAccessLayer partyAccessLayer = partyAccessLayer;

    private readonly QuestionAccessLayer questionAccessLayer = questionAccessLayer;

    private readonly ThemeAccessLayer themeAccessLayer = themeAccessLayer;

    /// <summary>
    ///     Event fired when party changed.
    /// </summary>
    public event EventHandler<PartyContext>? PartyChanged
    {
        add => this.context.PartyChanged += value;
        remove => this.context.PartyChanged -= value;
    }

    /// <summary>
    ///     Gets the parties.
    /// </summary>
    public IReadOnlyDictionary<Guid, PartyContext> Parties
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
    public async Task<PartyContext?> GetPartyAsync(Guid id, CancellationToken cancellationToken)
    {
        // Check if the party exists in the context
        if (this.context.Parties.TryGetValue(id, out var party))
            return party;

        // If not, retrieve the party from the database
        var dbParty = await this.partyAccessLayer.GetPartyAsync(id, cancellationToken);

        // If the party was not found in the database, return null
        // Otherwise, load the party into the context and return it
        return dbParty is null ? default : this.context.LoadParty(PartyContext.FromDbParty(dbParty));
    }

    /// <summary>
    ///     Asynchronously retrieves all parties from the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation. The result of the task is a dictionary mapping party IDs to party data.</returns>
    public async Task<Dictionary<Guid, PartyContext>> GetDbPartiesAsync(CancellationToken cancellationToken)
    {
        // Retrieve the parties from the database
        var dbParties = await this.partyAccessLayer.GetPartiesAsync(cancellationToken);

        // Convert the database parties to DTOs
        return PartyContext.FromDbParties(dbParties);
    }

    public async Task DeleteDbPartyAsync(Guid id, CancellationToken cancellationToken) => await this.partyAccessLayer.DeletePartyAsync(id, cancellationToken);

    public async Task LoadPartyAsync(Guid id, CancellationToken cancellationToken)
    {
        var partyFound = await this.partyAccessLayer.GetPartyAsync(id, cancellationToken);

        if (partyFound is null)
            return;

        _ = this.context.LoadParty(PartyContext.FromDbParty(partyFound));
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

    public async Task<IEnumerable<ThemeDto>> GetThemesAsync(Guid partyId)
    {
        if (this.Parties[partyId].Themes.Any())
            return this.context.Parties[partyId].Themes;

        var allThemes = await this.themeAccessLayer.GetAllThemesAsync();

        this.context.LoadThemes(
            partyId,
            allThemes.Select(
                t => new ThemeDto
                {
                    Id = t.Id,
                    Name = t.Name,
                }));

        return this.context.Parties[partyId].Themes;
    }

    public void SelectTheme(Guid partyId, Guid? teamId, ThemeDto theme)
        => this.context.SelectTheme(partyId, teamId, theme);

    public void ChoiceTheme(Guid partyId, Guid? teamId, ThemeDto theme)
        => this.context.ChoiceTheme(partyId, teamId, theme);

    public void ResetThemesChoices(Guid partyId)
        => this.context.ResetThemesChoices(partyId);

    public void HideTheme(Guid partyId)
    {
        this.Parties[partyId].ShowThemes = false;
        this.Parties[partyId].OnPartyChanged();
    }

    public void ShowTheme(Guid partyId)
    {
        this.Parties[partyId].ShowThemes = true;
        this.Parties[partyId].OnPartyChanged();
    }

    public void GenerateThemes(Guid partyId)
    {
        var currentParty = this.Parties[partyId];
        var random = new Random();
        var themes = new List<ThemeDto>();

        foreach (var (_, team) in currentParty.Teams)
        {
            var themesTeam = currentParty.Themes.Where(th => th.Team?.Id == team.Id).ToList();

            foreach (var themeTeam in themesTeam)
            {
                for (var i = 0; i < 5; i++)
                {
                    themes.Add(new() { Id = Guid.NewGuid(), Name = themeTeam.Name, Team = team });
                }
            }
        }

        var otherThemes = currentParty.Themes.Where(th => !themes.DistinctBy(theme => theme.Name).Select(theme => theme.Name).Contains(th.Name));

        var otherThemesLimited = new List<ThemeDto>();

        foreach (var otherTheme in otherThemes)
        {
            for (var i = 0; i < 5; i++)
            {
                otherThemesLimited.Add(new() { Id = Guid.NewGuid(), Name = otherTheme.Name, Team = null });
            }
        }

        themes.AddRange(themes.Count < 50 ? otherThemesLimited.Take(50 - themes.Count) : otherThemesLimited);

        var randomThemes = themes.OrderBy(x => random.Next());
        currentParty.LoadRandomThemes(randomThemes);
    }

    public void SetCurrentTimedPropositionResponse(Guid partyId, TimeOnly propositionDuration)
    {
        // TimeOnly to TimeSpan
        var timeSpan = new TimeSpan(propositionDuration.Hour, propositionDuration.Minute, propositionDuration.Second);
        this.context.SetCurrentResponse(partyId, ResponseType.TimedProposition, timeSpan);
    }

    public void ResumeTimedPropositionResponse(Guid partyId)
    {
        // TimeOnly to TimeSpan
        this.context.ResumeResponse(partyId, ResponseType.TimedProposition);
    }
}
