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

    public TeamDto(Guid id, string name)
        : this(id, name, new())
        => this.Score = 0;

    /// <summary>
    ///     Gets or sets the score.
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    ///     Gets or sets the buzz.
    /// </summary>
    public bool Buzz { get; set; }

    /// <summary>
    ///     Gets or sets the response.
    /// </summary>
    public string Response { get; set; }

    /// <summary>
    ///     Gets or sets the already qcm response.
    /// </summary>
    public bool AlreadyQcmResponse { get; set; }

    /// <summary>
    ///     Gets the answered.
    /// </summary>
    public bool Answered => this.Buzz || !string.IsNullOrWhiteSpace(this.Response);

    /// <summary>
    ///     Gets the party id.
    /// </summary>
    public Guid PartyId { get; set; }

    public static TeamDto FromDbTeam(Guid dbPartyId, Team dbTeam) => new(dbTeam.Id, dbTeam.Name) { Score = dbTeam.Score, PartyId = dbPartyId };

    public static Dictionary<Guid, TeamDto> FromDbTeams(Guid dbPartyId, IEnumerable<Team> dbTeams)
        => dbTeams.Select(team => FromDbTeam(dbPartyId, team)).ToDictionary(t => t.Id, t => t);

    public void AddPoint(int points) => this.Score += points;
}
