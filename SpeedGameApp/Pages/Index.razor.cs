namespace SpeedGameApp.Pages;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using SpeedGameApp.Business.Services;

public partial class Index
{
    private string partyName = string.Empty;

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

    private async void CreatePartyAsync(MouseEventArgs obj)
    {
        if (string.IsNullOrWhiteSpace(this.partyName))
            return;

        var cancellationTokenSource = new CancellationTokenSource();

        var guidParty = await this.GameService.CreatePartyAsync(this.partyName, cancellationTokenSource.Token);
        this.NavigationManager.NavigateTo($"/party/{guidParty}/admin");
    }
}
