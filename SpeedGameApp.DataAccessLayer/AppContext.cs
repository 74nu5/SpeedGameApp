namespace SpeedGameApp.DataAccessLayer;

using Microsoft.EntityFrameworkCore;

using SpeedGameApp.DataAccessLayer.Entities;

public class AppContext : DbContext
{
    public AppContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Party> Parties => this.Set<Party>();

    public DbSet<Team> Teams => this.Set<Team>();

    public DbSet<QcmQuestion> Questions => this.Set<QcmQuestion>();

    public DbSet<QcmTheme> QcmThemes => this.Set<QcmTheme>();

    public DbSet<Theme> Themes => this.Set<Theme>();
}
