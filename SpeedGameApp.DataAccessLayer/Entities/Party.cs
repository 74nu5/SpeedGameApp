namespace SpeedGameApp.DataAccessLayer.Entities;

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
    public string Name { get; set; } = default!;

    /// <summary>
    ///     Gets or sets the party teams.
    /// </summary>
    public ICollection<Team> Teams { get; set; } = new HashSet<Team>();
}
