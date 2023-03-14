namespace SpeedGameApp.DataAccessLayer.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

using SpeedGameApp.DataAccessLayer.AccessLayers;

/// <summary>
///     Class which represents the dal extensions.
/// </summary>
public static class DalExtensions
{
    /// <summary>
    ///     Adds the dal services.
    /// </summary>
    /// <param name="services">The services.</param>
    public static void AddDalServices(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();

        var environment = provider.GetRequiredService<IHostEnvironment>();
        var configuration = provider.GetRequiredService<IConfiguration>();

        _ = environment.IsDevelopment()
                ? services.AddDbContext<AppContext>(builder => builder.UseSqlite("Data Source=SpeedGameApp.db"))
                : services.AddDbContext<AppContext>(builder => builder.UseSqlServer(configuration.GetConnectionString("SpeedGameDb")));

        _ = provider.GetRequiredService<AppContext>().Database.EnsureCreated();

        services.TryAddTransient<PartyAccessLayer>();
        services.TryAddTransient<QuestionAccessLayer>();
        services.TryAddTransient<ThemeAccessLayer>();
    }
}
