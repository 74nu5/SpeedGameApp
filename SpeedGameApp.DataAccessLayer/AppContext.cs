namespace SpeedGameApp.DataAccessLayer;

using Microsoft.EntityFrameworkCore;

using SpeedGameApp.DataAccessLayer.Entities;

public class AppContext : DbContext
{
    /// <inheritdoc />
    public AppContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Party> Parties => this.Set<Party>();

    public DbSet<Team> Teams => this.Set<Team>();
    public DbSet<QcmQuestion> Questions => this.Set<QcmQuestion>();
    public DbSet<QcmTheme> Themes => this.Set<QcmTheme>();
}
