namespace SpeedGameApp.Pages;

using Microsoft.AspNetCore.Components;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services.Interfaces;

public class GamePageBase : ComponentBase, IDisposable
{
    protected CancellationTokenSource CancellationTokenSource = new();

    protected PartyDto CurrentParty = PartyDto.Empty;

    protected TeamDto CurrentTeam = TeamDto.Empty;

    /// <summary>
    ///     Gets or sets the party management service.
    /// </summary>
    [Inject]
    public IPartyManagementService PartyManagementService { get; set; } = default!;

    /// <summary>
    ///     Gets or sets the QCM service.
    /// </summary>
    [Inject]
    public IQcmService QcmService { get; set; } = default!;

    /// <summary>
    ///     Gets or sets the gameplay service.
    /// </summary>
    [Inject]
    public IGameplayService GameplayService { get; set; } = default!;

    /// <summary>
    ///     Gets or sets the theme service.
    /// </summary>
    [Inject]
    public IThemeService ThemeService { get; set; } = default!;

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
