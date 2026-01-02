namespace SpeedGameApp.Business.Data;

using SpeedGameApp.Business.Services.Models;
using SpeedGameApp.DataAccessLayer.Entities;
using SpeedGameApp.DataEnum;

/// <summary>
///     Record which represents a party.
/// </summary>
/// <param name="Id">The party id.</param>
/// <param name="Name">The party name.</param>
/// <param name="Teams">The party teams.</param>
public sealed record PartyDto(Guid Id, string Name, Dictionary<Guid, TeamDto> Teams)
{
    /// <summary>
    ///     Empty party.
    /// </summary>
    public static readonly PartyDto Empty = new(Guid.Empty, string.Empty);

    private List<ThemeDto> themes = [];

    private List<ThemeDto> randomThemes = [];

    /// <summary>
    ///     Initializes a new instance of the <see cref="PartyDto" /> class.
    /// </summary>
    /// <param name="id">The party id.</param>
    /// <param name="name">The party game.</param>
    public PartyDto(Guid id, string name)
            : this(id, name, [])
    {
    }

    /// <summary>
    ///     Event raised when the party changed.
    /// </summary>
    public event EventHandler? PartyChanged;

    /// <summary>
    ///     Event raised when the party reset.
    /// </summary>
    public event EventHandler? PartyReset;

    /// <summary>
    ///     Gets the teams sorted by score.
    /// </summary>
    public IEnumerable<TeamDto> TeamsSortedByScore => this.Teams.Select(kvp => kvp.Value).OrderByDescending(t => t.Score);

    /// <summary>
    ///     Gets the current response type.
    /// </summary>
    public ResponseType CurrentResponseType { get; internal set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the themes displayed.
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
    ///     Method to raise the <see cref="PartyReset" /> event.
    /// </summary>
    internal void OnPartyReset()
        => this.PartyReset?.Invoke(this, EventArgs.Empty);
}
