namespace SpeedGameApp.Pages;

using Microsoft.AspNetCore.Components;
using SpeedGameApp.Business.Context;
using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services;

public class GamePageBase : ComponentBase, IDisposable
{
    protected CancellationTokenSource CancellationTokenSource = new();

    protected PartyContext CurrentParty = PartyContext.Empty;

    protected TeamDto CurrentTeam = TeamDto.Empty;

    /// <summary>
    ///     Gets or sets the game service.
    /// </summary>
    [Inject]
    public GameService GameService { get; set; } = default!;

    /// <summary>
    ///     Gets or sets the navigation manager.
    /// </summary>
    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        this.CancellationTokenSource.Cancel();
        this.CancellationTokenSource.Dispose();
    }
}
