namespace SpeedGameApp.Business.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services;
using SpeedGameApp.Business.Services.Implementations;
using SpeedGameApp.Business.Services.Interfaces;
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

        // Core services
        services.TryAddTransient<GameService>();
        services.TryAddTransient<CsvService>();
        services.TryAddSingleton(TimeProvider.System); // .NET 8+ TimeProvider pour testabilité

        // Party management services (separated responsibilities)
        services.TryAddSingleton<IPartyEventPublisher, PartyEventPublisher>();
        services.TryAddSingleton<IPartyRepository, PartyRepository>();
        services.TryAddSingleton<IPartyStateManager, PartyStateManager>();
        services.TryAddSingleton<IThemeManager, ThemeManager>();

        // Legacy (kept for backwards compatibility, will be removed)
        services.TryAddSingleton<PartyContext>();
    }
}
