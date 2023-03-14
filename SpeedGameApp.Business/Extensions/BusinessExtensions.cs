namespace SpeedGameApp.Business.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services;
using SpeedGameApp.DataAccessLayer.Extensions;

/// <summary>
///     Extension methods for business services.
/// </summary>
public static class BusinessExtensions
{
    /// <summary>
    ///     Add business services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static void AddBusinessServices(this IServiceCollection services)
    {
        services.AddDalServices();
        services.TryAddTransient<GameService>();
        services.TryAddTransient<CsvService>();
        services.TryAddSingleton<PartyContext>();
    }
}
