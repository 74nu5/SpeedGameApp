namespace SpeedGameApp.Business.Context;

using System.Collections.Concurrent;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services.Models;
using SpeedGameApp.DataAccessLayer.Entities;
using SpeedGameApp.DataEnum;

/// <summary>
///     The party context.
/// </summary>
internal sealed class PartiesContext
{
    private readonly IDictionary<Guid, PartyContext> parties = new ConcurrentDictionary<Guid, PartyContext>();

    /// <summary>
    ///     Event raised when the party changed.
    /// </summary>
    public event EventHandler<PartyContext>? PartyChanged;

    /// <summary>
    ///     Gets the parties.
    /// </summary>
    public IReadOnlyDictionary<Guid, PartyContext> Parties
        => this.parties.AsReadOnly();

    /// <summary>
    ///     Delete a party.
    /// </summary>
    /// <param name="id">The party id to delete.</param>
    public void DeleteParty(Guid id) => this.parties.Remove(id);

    /// <summary>
    ///     Determines whether the party exists.
    /// </summary>
    /// <param name="id">The party id.</param>
    /// <returns><c>true</c> if the party exists; otherwise, <c>false</c>.</returns>
    public bool ExistsParty(Guid id) => this.Parties.ContainsKey(id);

    /// <summary>
    ///     Add points to a team.
    /// </summary>
    /// <param name="teamDto">The team to add points.</param>
    /// <param name="points">The points to add.</param>
    public void AddPoints(TeamDto teamDto, int points)
    {
        var party = this.parties.FirstOrDefault(kvp => kvp.Value.Teams.Any(t => t.Key == teamDto.Id)).Value;
        teamDto.AddPoint(points);
        party.OnPartyChanged();
    }

    /// <summary>
    ///     Delete all parties.
    /// </summary>
    public void DeleteAllParties() => this.parties.Clear();

    /// <summary>
    ///     Set the current response.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <param name="responseType">The response type.</param>
    /// <param name="propositionDuration">The proposition duration.</param>
    public void SetCurrentResponse(Guid partyId, ResponseType responseType, TimeSpan? propositionDuration = default)
        => this.parties[partyId].StartResponse(responseType, propositionDuration);

    /// <summary>
    ///     Buzz for a team.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <param name="teamId">The team id.</param>
    public void BuzzTeam(Guid partyId, Guid teamId)
    {
        var (_, team) = this.parties[partyId].Teams.FirstOrDefault(pair => pair.Key == teamId);

        if (!this.parties[partyId].HasResponse)
        {
            team.Buzz = true;
            this.parties[partyId].HasResponse = true;
        }

        this.parties[partyId].OnPartyChanged();
    }

    /// <summary>
    ///     Method to reset the party.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    public void ResetTeam(Guid partyId)
    {
        this.parties[partyId].HasResponse = false;

        foreach (var (_, team) in this.parties[partyId].Teams)
        {
            team.Buzz = false;
            team.Response = string.Empty;
        }

        this.parties[partyId].OnPartyReset();
        this.parties[partyId].OnPartyChanged();
    }

    /// <summary>
    ///     Method to proposition from team.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <param name="teamId">The team id.</param>
    /// <param name="response">The team response.</param>
    public void PropositionTeam(Guid partyId, Guid teamId, string response)
    {
        var (_, team) = this.parties[partyId].Teams.FirstOrDefault(pair => pair.Key == teamId);

        if (!this.parties[partyId].HasResponse)
        {
            team.Response = response;
            this.parties[partyId].HasResponse = true;
            this.parties[partyId].PauseTimer();
        }

        this.parties[partyId].OnPartyChanged();
    }

    /// <summary>
    ///     Method to load a party.
    /// </summary>
    /// <param name="partyContext">Party to load.</param>
    /// <returns>Returns the new party.</returns>
    public PartyContext LoadParty(PartyContext partyContext)
    {
        this.parties.Add(partyContext.Id, partyContext);
        return partyContext;
    }

    /// <summary>
    ///     Method to remove a team.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <param name="teamId">The team id.</param>
    public void RemoveTeam(Guid partyId, Guid teamId)
    {
        _ = this.parties[partyId].Teams.Remove(teamId);
        this.parties[partyId].OnPartyChanged();
    }

    /// <summary>
    ///     Method to set the current qcm.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <param name="question">The qcm.</param>
    public void SetCurrentQcm(Guid partyId, QcmQuestionDto question)
    {
        var party = this.Parties[partyId];
        party.CurrentQcm = question;
        party.OnPartyChanged();
    }

    /// <summary>
    ///     Method to get the current qcm response.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <param name="teamId">The team id.</param>
    /// <param name="response">The qcm response.</param>
    public void PropositionQcmTeam(Guid partyId, Guid teamId, string response)
    {
        var (_, team) = this.parties[partyId].Teams.FirstOrDefault(pair => pair.Key == teamId);

        if (!team.AlreadyQcmResponse)
        {
            var party = this.Parties[partyId];

            team.Response = response;
            //team.AlreadyQcmResponse = true;
            team.QcmValidResponse = team.Response == party.CurrentQcm?.Response;
        }

        this.parties[partyId].OnPartyChanged();
    }

    /// <summary>
    ///     Method to add party.
    /// </summary>
    /// <param name="guidParty">The party id.</param>
    /// <param name="partyName">Party name.</param>
    public void AddParty(Guid guidParty, string partyName)
    {
        var party = new PartyContext(guidParty, partyName);
        this.parties.Add(guidParty, party);
        this.OnPartyChanged(party);
    }

    /// <summary>
    ///     Method to add team to a party.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <param name="team">The new team.</param>
    /// <returns>Returns the team id.</returns>
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

    /// <summary>
    ///     Method to raise the party changed event.
    /// </summary>
    /// <param name="e">The party changed.</param>
    public void OnPartyChanged(PartyContext e)
        => this.PartyChanged?.Invoke(this, e);

    /// <summary>
    ///     Method to load themesDtos.
    /// </summary>
    /// <param name="partyId">The party id.</param>
    /// <param name="themesDtos">The themesDtos to load.</param>
    public void LoadThemes(Guid partyId, IEnumerable<ThemeDto> themesDtos)
        => this.parties[partyId].LoadThemes(themesDtos.ToList());

    public void SelectTheme(Guid partyId, Guid? teamId, ThemeDto theme)
    {
        var themeDto = this.parties[partyId].RandomThemes.FirstOrDefault(t => t.Id == theme.Id);

        if (themeDto is null)
            return;

        themeDto.AlreadyTaken = true;
        this.parties[partyId].OnPartyChanged();
    }

    public void ChoiceTheme(Guid partyId, Guid? teamId, ThemeDto theme)
    {
        var themeDto = this.parties[partyId].Themes.FirstOrDefault(t => t.Id == theme.Id);

        if (themeDto is null || teamId is null)
            return;

        themeDto.Team = this.parties[partyId].Teams[(Guid)teamId];
        this.parties[partyId].OnPartyChanged();
    }

    public void ResetThemesChoices(Guid partyId)
    {
        foreach (var theme in this.parties[partyId].Themes)
        {
            theme.Team = null;
        }

        this.parties[partyId].OnPartyChanged();
    }

    public void ResumeResponse(Guid partyId, ResponseType timedProposition)
    {
        foreach (var (_, team) in this.parties[partyId].Teams)
        {
            team.Response = string.Empty;
        }

        this.parties[partyId].HasResponse = false;
        this.parties[partyId].OnPartyChanged();
        this.parties[partyId].ResumeTimer();
    }
}
