namespace SpeedGameApp.Pages;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using SpeedGameApp.Business.Services.Interfaces;

public partial class Index
{
    private string partyName = string.Empty;

    /// <summary>
    ///     Gets or sets the party management service.
    /// </summary>
    [Inject]
    public IPartyManagementService PartyManagementService { get; set; } = default!;

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

        var result = await this.PartyManagementService.CreatePartyAsync(this.partyName, cancellationTokenSource.Token);

        if (result.IsSuccess)
            this.NavigationManager.NavigateTo($"/party/{result.Value}/admin");
        // TODO: Handle error case - could display error message to user
    }
}
