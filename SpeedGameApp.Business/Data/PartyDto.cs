﻿namespace SpeedGameApp.Business.Data;

using System.Timers;

using SpeedGameApp.Business.Services.Models;
using SpeedGameApp.DataAccessLayer.Entities;
using SpeedGameApp.DataEnum;

/// <summary>
///     Record which represents a party.
/// </summary>
public sealed record PartyDto
{
    /// <summary>
    ///     Empty party.
    /// </summary>
    public static readonly PartyDto Empty = new(Guid.Empty, string.Empty);

    private readonly Timer timerSecond = new(TimeSpan.FromSeconds(1)) { Enabled = false, AutoReset = true };

    private List<ThemeDto> themes = new();

    private List<ThemeDto> randomThemes = new();

    private Timer? timerProposition;

    private double elapsedTime;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PartyDto" /> class.
    /// </summary>
    /// <param name="id">The party id.</param>
    /// <param name="name">The party game.</param>
    public PartyDto(Guid id, string name)
        : this(id, name, new())
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PartyDto" /> class.
    /// </summary>
    /// <param name="id">The party id.</param>
    /// <param name="name">The party name.</param>
    /// <param name="teams">The party teams.</param>
    public PartyDto(Guid id, string name, Dictionary<Guid, TeamDto> teams)
    {
        this.Id = id;
        this.Name = name;
        this.Teams = teams;
        this.timerSecond.Elapsed += this.OnTickTimer;
    }

    /// <summary>
    ///     Event raised when the party changed.
    /// </summary>
    public event EventHandler? PartyChanged;

    /// <summary>
    ///     Event raised when the party reset.
    /// </summary>
    public event EventHandler? PartyReset;

    public event EventHandler? ResponseStarted;

    public event EventHandler<bool>? TickTimer;

    public event EventHandler? TimerEnd;

    public TimeSpan Interval
        => TimeSpan.FromMilliseconds(this.timerProposition?.Interval ?? 0);

    /// <summary>
    ///     Gets the teams sorted by score.
    /// </summary>
    public IEnumerable<TeamDto> TeamsSortedByScore => this.Teams.Select(kvp => kvp.Value).OrderByDescending(t => t.Score);

    /// <summary>
    ///     Gets the current response type.
    /// </summary>
    public ResponseType CurrentResponseType { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether the themes displayed.
    /// </summary>
    public bool ShowThemes { get; set; }

    /// <summary>
    ///     Gets a value indicating whether the party already response.
    /// </summary>
    public bool AlreadyResponse { get; internal set; }

    /// <summary>
    ///     Gets the current qcm.
    /// </summary>
    public QcmQuestionDto? CurrentQcm { get; internal set; }

    public TimeSpan ElapsedTime
        => TimeSpan.FromMilliseconds(this.elapsedTime);

    /// <summary>
    ///     Gets the parties.
    /// </summary>
    public IReadOnlyList<ThemeDto> Themes
        => this.themes.AsReadOnly();

    /// <summary>
    ///     Gets the parties.
    /// </summary>
    public IReadOnlyList<ThemeDto> RandomThemes
        => this.randomThemes.AsReadOnly();

    /// <summary>
    ///     Gets a value indicating whether the timer is enabled.
    /// </summary>
    public bool IsTimerEnabled => this.timerProposition?.Enabled ?? false;

    /// <summary>The party id.</summary>
    public Guid Id { get; init; }

    /// <summary>The party name.</summary>
    public string Name { get; init; }

    /// <summary>The party teams.</summary>
    public Dictionary<Guid, TeamDto> Teams { get; init; }

    /// <summary>
    ///     Method to load themesDtos.
    /// </summary>
    /// <param name="themesDtos">The themesDtos to load.</param>
    public void LoadThemes(IEnumerable<ThemeDto> themesDtos)
        => this.themes = themesDtos.ToList();

    /// <summary>
    ///     Method to load themesDtos.
    /// </summary>
    /// <param name="themesDtos">The themesDtos to load.</param>
    public void LoadRandomThemes(IEnumerable<ThemeDto> themesDtos)
        => this.randomThemes = themesDtos.ToList();

    public void StartTimer(TimeSpan timeSpan)
    {
        this.timerProposition = new(timeSpan);
        this.timerProposition.Elapsed += this.TimerPropositionElapsed;
        this.elapsedTime = this.timerProposition.Interval;
        this.timerProposition.Start();
        this.timerSecond.Start();
    }

    public void StartResponse(ResponseType responseType, TimeSpan? propositionDuration)
    {
        this.CurrentResponseType = responseType;

        if (responseType == ResponseType.TimedProposition && propositionDuration.HasValue)
        {
            this.StartTimer(propositionDuration.Value);
        }

        this.OnResponseStarted();
        this.OnPartyChanged();
    }

    public void Deconstruct(out Guid id, out string name, out Dictionary<Guid, TeamDto> teams)
    {
        id = this.Id;
        name = this.Name;
        teams = this.Teams;
    }

    /// <summary>
    ///     Method to get <see cref="PartyDto" /> from <see cref="Party" />.
    /// </summary>
    /// <param name="dbParty">The party to transform.</param>
    /// <returns>Return the new party.</returns>
    internal static PartyDto FromDbParty(Party dbParty)
        => new(dbParty.Id, dbParty.Name, TeamDto.FromDbTeams(dbParty.Id, dbParty.Teams));

    /// <summary>
    ///     Method to get <see cref="PartyDto" /> dictionary from <see cref="Party" /> list.
    /// </summary>
    /// <param name="dbParties">The parties to transform.</param>
    /// <returns>Returns the new parties dictionary.</returns>
    internal static Dictionary<Guid, PartyDto> FromDbParties(IEnumerable<Party> dbParties)
        => dbParties.Select(FromDbParty).ToDictionary(p => p.Id, p => p);

    /// <summary>
    ///     Method to raise the <see cref="PartyChanged" /> event.
    /// </summary>
    internal void OnPartyChanged()
        => this.PartyChanged?.Invoke(this, EventArgs.Empty);

    /// <summary>
    ///     Method to raise the <see cref="ResponseStarted" /> event.
    /// </summary>
    internal void OnResponseStarted()
        => this.ResponseStarted?.Invoke(this, EventArgs.Empty);

    /// <summary>
    ///     Method to raise the <see cref="PartyChanged" /> event.
    /// </summary>
    internal void OnTimerEnd()
        => this.TimerEnd?.Invoke(this, EventArgs.Empty);

    /// <summary>
    ///     Method to raise the <see cref="PartyReset" /> event.
    /// </summary>
    internal void OnPartyReset()
        => this.PartyReset?.Invoke(this, EventArgs.Empty);

    private void TimerPropositionElapsed(object? state, ElapsedEventArgs elapsedEventArgs)
    {
        this.timerProposition?.Stop();
        this.timerProposition?.Dispose();
        this.timerProposition = null;
        this.timerSecond.Stop();
        this.OnTimerEnd();
        this.OnPartyChanged();
    }

    private void OnTickTimer(object? sender, EventArgs e)
    {
        this.elapsedTime -= 1000;
        this.TickTimer?.Invoke(this, this.IsTimerEnabled);
    }
}
