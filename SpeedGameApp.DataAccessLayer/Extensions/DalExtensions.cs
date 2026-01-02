namespace SpeedGameApp.DataAccessLayer.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

using SpeedGameApp.DataAccessLayer.AccessLayers;
using SpeedGameApp.DataAccessLayer.Interfaces;

/// <summary>
///     Extension methods for Data Access Layer services.
/// </summary>
public static class DalExtensions
{
    /// <summary>
    ///     Adds the data access layer services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static void AddDalServices(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();

        var environment = provider.GetRequiredService<IHostEnvironment>();
        var configuration = provider.GetRequiredService<IConfiguration>();

        // Register DbContext
        _ = environment.IsDevelopment()
                ? services.AddDbContext<SpeedGameDbContext>(builder => builder.UseSqlite("Data Source=SpeedGameApp.db"))
                : services.AddDbContext<SpeedGameDbContext>(builder => builder.UseSqlServer(configuration.GetConnectionString("SpeedGameDb")));

        // Register access layers with their interfaces
        services.TryAddScoped<IPartyAccessLayer, PartyAccessLayer>();
        services.TryAddScoped<IQuestionAccessLayer, QuestionAccessLayer>();
        services.TryAddScoped<IThemeAccessLayer, ThemeAccessLayer>();
    }
}
