namespace SpeedGameApp.Business.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services;
using SpeedGameApp.DataAccessLayer.Extensions;

public static class BusinessExtensions
{
    public static void AddBusinessServices(this IServiceCollection services)
    {
        services.AddDalServices();
        services.TryAddTransient<GameService>();
        services.TryAddTransient<CsvService>();
        services.TryAddSingleton<PartyContext>();

    }
}
