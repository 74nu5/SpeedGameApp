namespace SpeedGameApp.DataAccessLayer.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using SpeedGameApp.DataAccessLayer.AccessLayers;

public static class DalExtensions
{
    public static void AddDalServices(this IServiceCollection services)
    {
        _ = services.AddDbContext<AppContext>(builder => builder.UseSqlite("Data Source=SpeedGameApp.db"));

        _ = services.BuildServiceProvider().GetRequiredService<AppContext>().Database.EnsureCreated();

        services.TryAddTransient<PartyAccessLayer>();
        services.TryAddTransient<QuestionAccessLayer>();
    }
}
