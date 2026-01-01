namespace SpeedGameApp.Business.Data;

using SpeedGameApp.DataAccessLayer.Entities;

/// <summary>
///     Record which represents a team.
/// </summary>
/// <param name="Id">The team id.</param>
/// <param name="Name">The team name.</param>
/// <param name="Players">The team players.</param>
public sealed record TeamDto(Guid Id, string Name, List<Player> Players)
{
    /// <summary>
    ///     The empty team.
    /// </summary>
    public static readonly TeamDto Empty = new(Guid.Empty, string.Empty);

    /// <summary>
    ///     Initializes a new instance of the <see cref="TeamDto" /> class.
    /// </summary>
    /// <param name="id">The team id.</param>
    /// <param name="name">The team name.</param>
    public TeamDto(Guid id, string name)
        : this(id, name, [])
        => this.Score = 0;

    /// <summary>
    ///     Gets the score.
    /// </summary>
    public int Score { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether the buzz.
    /// </summary>
    public bool Buzz { get; internal set; }

    /// <summary>
    ///     Gets or sets the response.
    /// </summary>
    public string Response { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets a value indicating whether the already qcm response.
    /// </summary>
    public bool AlreadyQcmResponse { get; set; }

    /// <summary>
    ///     Gets a value indicating whether the answered.
    /// </summary>
    public bool Answered => this.Buzz || !string.IsNullOrWhiteSpace(this.Response);

    /// <summary>
    ///     Gets the party id.
    /// </summary>
    public Guid PartyId { get; init; }

    /// <summary>
    ///     Gets or sets a value indicating whether the qcm valid response.
    /// </summary>
    public bool? QcmValidResponse { get; set; }

    /// <summary>
    ///     Method to convert a db team to a team dto.
    /// </summary>
    /// <param name="dbPartyId">The db party id.</param>
    /// <param name="dbTeam">The db team.</param>
    /// <returns>Returns the team dto.</returns>
    public static TeamDto FromDbTeam(Guid dbPartyId, Team dbTeam) => new(dbTeam.Id, dbTeam.Name) { Score = dbTeam.Score, PartyId = dbPartyId };

    /// <summary>
    ///     Method to convert a list of db teams to a dictionary of team dto.
    /// </summary>
    /// <param name="dbPartyId">The db party id.</param>
    /// <param name="dbTeams">The db teams.</param>
    /// <returns>Returns the team dictionary.</returns>
    public static Dictionary<Guid, TeamDto> FromDbTeams(Guid dbPartyId, IEnumerable<Team> dbTeams)
        => dbTeams.Select(team => FromDbTeam(dbPartyId, team)).ToDictionary(t => t.Id, t => t);

    /// <summary>
    ///     Method to add points to the team.
    /// </summary>
    /// <param name="points">The point to add.</param>
    public void AddPoint(int points) => this.Score += points;
}
