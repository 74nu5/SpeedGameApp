namespace SpeedGameApp.Business.Extensions;

using FluentValidation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using SpeedGameApp.Business.Services;
using SpeedGameApp.Business.Services.Implementations;
using SpeedGameApp.Business.Services.Interfaces;
using SpeedGameApp.Business.Validators;
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

        // Validators
        _ = services.AddValidatorsFromAssemblyContaining<PartyNameValidator>(ServiceLifetime.Singleton);

        // Specialized business services (following SRP)
        services.TryAddTransient<IPartyManagementService, PartyManagementService>();
        services.TryAddTransient<IQcmService, QcmService>();
        services.TryAddTransient<IGameplayService, GameplayService>();
        services.TryAddTransient<IThemeService, ThemeService>();

        // Core services
        services.TryAddTransient<CsvService>();
        services.TryAddSingleton(TimeProvider.System); // .NET 8+ TimeProvider pour testabilité

        // Party management infrastructure (separated responsibilities)
        services.TryAddSingleton<IPartyEventPublisher, PartyEventPublisher>();
        services.TryAddSingleton<IPartyRepository, PartyRepository>();
        services.TryAddSingleton<IPartyStateManager, PartyStateManager>();
        services.TryAddSingleton<IThemeManager, ThemeManager>();
    }
}
