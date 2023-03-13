namespace SpeedGameApp.DataAccessLayer.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        _ = services.AddDbContext<AppContext>(builder => builder.UseSqlite("Data Source=SpeedGameApp.db"));

        _ = services.BuildServiceProvider().GetRequiredService<AppContext>().Database.EnsureCreated();

        services.TryAddTransient<PartyAccessLayer>();
        services.TryAddTransient<QuestionAccessLayer>();
        services.TryAddTransient<ThemeAccessLayer>();
    }
}
