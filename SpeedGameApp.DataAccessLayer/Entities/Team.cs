namespace SpeedGameApp.DataAccessLayer.Entities;

using System.ComponentModel.DataAnnotations;

/// <summary>
///     Class which represent a team.
/// </summary>
public sealed class Team
{
    /// <summary>
    ///     Gets or sets the team id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the score.
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    ///     Gets or sets the team name.
    /// </summary>
    [MaxLength(250)]
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets the party id.
    /// </summary>
    public Guid PartyId { get; set; }

    /// <summary>
    ///     Gets or sets the party.
    /// </summary>
    public Party Party { get; set; } = default!;
}
