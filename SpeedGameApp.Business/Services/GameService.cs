namespace SpeedGameApp.Business.Services;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services.Interfaces;
using SpeedGameApp.Business.Services.Models;
using SpeedGameApp.DataAccessLayer.AccessLayers;
using SpeedGameApp.DataEnum;

/// <summary>
///     Class which defines all game mathods.
/// </summary>
public sealed class GameService(
    IPartyRepository partyRepository,
    IPartyStateManager stateManager,
    IPartyEventPublisher eventPublisher,
    IThemeManager themeManager,
    PartyAccessLayer partyAccessLayer,
    QuestionAccessLayer questionAccessLayer,
    ThemeAccessLayer themeAccessLayer,
    TimeProvider timeProvider)
{
    /// <summary>
    ///     Event fired when party changed.
    /// </summary>
    public event EventHandler<PartyDto>? PartyChanged
    {
        add => eventPublisher.PartyChanged += value;
        remove => eventPublisher.PartyChanged -= value;
    }

    /// <summary>
    ///     Gets the parties.
    /// </summary>
    public IReadOnlyDictionary<Guid, PartyDto> Parties
        => partyRepository.Parties;

    /// <summary>
    ///     Asynchronously creates a new party with the specified name.
    /// </summary>
    /// <param name="partyName">The name of the party to create.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation. The result of the task is the ID of the newly created party.</returns>
    public async Task<Guid> CreatePartyAsync(string partyName, CancellationToken cancellationToken)
    {
        // Create the party in the database
        var party = await partyAccessLayer.CreatePartyAsync(partyName, cancellationToken);

        // Add the party to the repository
        partyRepository.AddParty(party.Id, partyName);

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
        if (!partyRepository.ExistsParty(partyId))
            return default;

        // Create team party and return default value if operation is unsuccessful
        var team = await partyAccessLayer.CreateTeamPartyAsync(partyId, teamName, cancellationToken);

        if (team is null)
            return default;

        // Add team to repository and return its ID
        _ = partyRepository.AddTeamParty(partyId, team);
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
        // Check if the party exists in the repository
        if (!partyRepository.Parties.TryGetValue(id, out var party))
            return;

        // Loop through each team in the party and update their score
        foreach (var (key, team) in party.Teams)
            await partyAccessLayer.UpdateScoreAsync(key, team.Score, cancellationToken);
    }

    /// <summary>
    ///     Asynchronously retrieves a party with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the party to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation. The result of the task is the party with the specified ID, or null if the party was not found.</returns>
    public async Task<PartyDto?> GetPartyAsync(Guid id, CancellationToken cancellationToken)
    {
        // Check if the party exists in the repository
        if (partyRepository.Parties.TryGetValue(id, out var party))
            return party;

        // If not, retrieve the party from the database
        var dbParty = await partyAccessLayer.GetPartyAsync(id, cancellationToken);

        // If the party was not found in the database, return null
        // Otherwise, load the party into the repository and return it
        return dbParty is null ? default : partyRepository.LoadParty(PartyDto.FromDbParty(dbParty));
    }

    /// <summary>
    ///     Asynchronously retrieves all parties from the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation. The result of the task is a dictionary mapping party IDs to party data.</returns>
    public async Task<Dictionary<Guid, PartyDto>> GetDbPartiesAsync(CancellationToken cancellationToken)
    {
        // Retrieve the parties from the database
        var dbParties = await partyAccessLayer.GetPartiesAsync(cancellationToken);

        // Convert the database parties to DTOs
        return PartyDto.FromDbParties(dbParties);
    }

    public async Task DeleteDbPartyAsync(Guid id, CancellationToken cancellationToken)
        => await partyAccessLayer.DeletePartyAsync(id, cancellationToken);

    public async Task LoadPartyAsync(Guid id, CancellationToken cancellationToken)
    {
        var partyFound = await partyAccessLayer.GetPartyAsync(id, cancellationToken);

        if (partyFound is null)
            return;

        _ = partyRepository.LoadParty(PartyDto.FromDbParty(partyFound));
    }

    public async Task DeleteTeamAsync(Guid partyId, Guid teamId, CancellationToken cancellationToken)
    {
        await partyAccessLayer.DeleteTeamAsync(partyId, teamId, cancellationToken);
        partyRepository.RemoveTeam(partyId, teamId);
    }

    public void SetRandomQcm(Guid partyId)
    {
        var question = questionAccessLayer.GetRandom();
        stateManager.SetCurrentQcm(partyId, QcmQuestionDto.FromQcmQuestion(question));
    }

    public async Task AddPointsAsync(TeamDto teamDto, int points, CancellationToken cancellationToken)
    {
        stateManager.AddPoints(teamDto, points);
        await this.SavePartyAsync(teamDto.PartyId, cancellationToken);
    }

    public void BuzzTeam(Guid partyId, Guid teamId)
        => stateManager.BuzzTeam(partyId, teamId);

    public void DeleteAllParties()
        => partyRepository.DeleteAllParties();

    public void DeleteParty(Guid id)
        => partyRepository.DeleteParty(id);

    public void PropositionQcmTeam(Guid partyId, Guid teamId, string response)
        => stateManager.PropositionQcmTeam(partyId, teamId, response);

    public void PropositionTeam(Guid partyId, Guid teamId, string response)
        => stateManager.PropositionTeam(partyId, teamId, response);

    public void ResetTeam(Guid partyId)
        => stateManager.ResetTeam(partyId);

    public void SetCurrentResponse(Guid partyId, ResponseType responseType)
        => stateManager.SetCurrentResponse(partyId, responseType);

    public async Task<IEnumerable<ThemeDto>> GetThemesAsync(Guid partyId)
    {
        if (this.Parties[partyId].Themes.Any())
            return partyRepository.Parties[partyId].Themes;

        var allThemes = await themeAccessLayer.GetAllThemesAsync();

        themeManager.LoadThemes(partyId,
                                allThemes.Select(t => new ThemeDto
                                {
                                    Id = t.Id,
                                    Name = t.Name,
                                }));

        return partyRepository.Parties[partyId].Themes;
    }

    public void SelectTheme(Guid partyId, Guid? teamId, ThemeDto theme)
        => themeManager.SelectTheme(partyId, teamId, theme);

    public void ChoiceTheme(Guid partyId, Guid? teamId, ThemeDto theme)
        => themeManager.ChoiceTheme(partyId, teamId, theme);

    public void ResetThemesChoices(Guid partyId)
        => themeManager.ResetThemesChoices(partyId);

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
        var random = Random.Shared; // .NET 6+ amélioration - thread-safe random partagé
        List<ThemeDto> themes = [];

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

        var selectedThemeNames = themes.DistinctBy(theme => theme.Name).Select(theme => theme.Name);
        var otherThemes = currentParty.Themes.Where(th => !selectedThemeNames.Contains(th.Name));

        List<ThemeDto> otherThemesLimited = [];
        foreach (var otherTheme in otherThemes)
        {
            for (var i = 0; i < 5; i++)
            {
                otherThemesLimited.Add(new() { Id = Guid.NewGuid(), Name = otherTheme.Name, Team = null });
            }
        }

        themes.AddRange(themes.Count < 50 ? otherThemesLimited.Take(50 - themes.Count) : otherThemesLimited);

        var randomThemes = themes.OrderBy(_ => random.Next());
        currentParty.LoadRandomThemes(randomThemes);
    }
}
