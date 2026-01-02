namespace SpeedGameApp.Pages;

using Microsoft.AspNetCore.Components;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services.Interfaces;

/// <summary>
///     Base class for the game page, providing required services and data.
/// </summary>
public class GamePageBase : ComponentBase, IDisposable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GamePageBase" /> class with the required services.
    /// </summary>
    /// <param name="partyManagementService">The party management service.</param>
    /// <param name="qcmService">The QCM management service.</param>
    /// <param name="gameplayService">The gameplay management service.</param>
    /// <param name="themeService">The theme management service.</param>
    /// <param name="navigationManager">The navigation manager.</param>
    public GamePageBase(IPartyManagementService partyManagementService, IQcmService qcmService, IGameplayService gameplayService, IThemeService themeService, NavigationManager navigationManager)
    {
        this.PartyManagementService = partyManagementService;
        this.QcmService = qcmService;
        this.GameplayService = gameplayService;
        this.ThemeService = themeService;
        this.NavigationManager = navigationManager;
    }

    /// <summary>
    ///     Gets or sets Cancellation token source used to manage the cancellation of asynchronous operations.
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource { get; set; } = new();

    /// <summary>
    ///     Gets or sets represents the current party.
    /// </summary>
    protected PartyDto CurrentParty { get; set; } = PartyDto.Empty;

    /// <summary>
    ///     Gets or sets represents the current team.
    /// </summary>
    protected TeamDto CurrentTeam { get; set; } = TeamDto.Empty;

    /// <summary>
    ///     Gets the party management service.
    /// </summary>
    protected IPartyManagementService PartyManagementService { get; }

    /// <summary>
    ///     Gets the QCM management service.
    /// </summary>
    protected IQcmService QcmService { get; }

    /// <summary>
    ///     Gets the gameplay management service.
    /// </summary>
    protected IGameplayService GameplayService { get; }

    /// <summary>
    ///     Gets the theme management service.
    /// </summary>
    protected IThemeService ThemeService { get; }

    /// <summary>
    ///     Gets the navigation manager.
    /// </summary>
    protected NavigationManager NavigationManager { get; }

    /// <summary>
    ///     Releases the resources used by the <see cref="GamePageBase" /> instance.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        this.CancellationTokenSource.Cancel();
        this.CancellationTokenSource.Dispose();
    }
}
