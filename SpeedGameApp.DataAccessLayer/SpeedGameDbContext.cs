namespace SpeedGameApp.DataAccessLayer;

using Microsoft.EntityFrameworkCore;

using SpeedGameApp.DataAccessLayer.Entities;

/// <summary>
///     Database context for the SpeedGame application.
/// </summary>
public class SpeedGameDbContext : DbContext
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SpeedGameDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public SpeedGameDbContext(DbContextOptions<SpeedGameDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    ///     Gets the parties DbSet.
    /// </summary>
    public DbSet<Party> Parties => this.Set<Party>();

    /// <summary>
    ///     Gets the teams DbSet.
    /// </summary>
    public DbSet<Team> Teams => this.Set<Team>();

    /// <summary>
    ///     Gets the QCM questions DbSet.
    /// </summary>
    public DbSet<QcmQuestion> Questions => this.Set<QcmQuestion>();

    /// <summary>
    ///     Gets the QCM themes DbSet.
    /// </summary>
    public DbSet<QcmTheme> QcmThemes => this.Set<QcmTheme>();

    /// <summary>
    ///     Gets the blind test themes DbSet.
    /// </summary>
    public DbSet<Theme> Themes => this.Set<Theme>();
}
