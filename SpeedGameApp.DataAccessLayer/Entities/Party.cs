namespace SpeedGameApp.DataAccessLayer.Entities;

using System.ComponentModel.DataAnnotations;

/// <summary>
///     Class which represent a party.
/// </summary>
public sealed class Party
{
    /// <summary>
    ///     Gets or sets the party id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the party name.
    /// </summary>
    [MaxLength(250)]
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets the party teams.
    /// </summary>
    public ICollection<Team> Teams { get; set; } = [];
}
