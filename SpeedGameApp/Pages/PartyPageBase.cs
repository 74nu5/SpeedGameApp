namespace SpeedGameApp.Pages;

using Microsoft.AspNetCore.Components;

using SpeedGameApp.Business.Data;

/// <summary>
///     The party page base.
/// </summary>
public class PartyPageBase : GamePageBase
{
    /// <summary>
    ///     Gets or sets the current party id.
    /// </summary>
    [Parameter]
    public Guid PartyId { get; set; }

    /// <summary>
    ///     Gets or sets the current team id.
    /// </summary>
    [Parameter]
    public Guid? TeamId { get; set; }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        this.CurrentParty = await this.PartyManagementService.GetPartyAsync(this.PartyId, this.CancellationTokenSource.Token) ?? PartyDto.Empty;

        if (this.CurrentParty == PartyDto.Empty)
        {
            this.NavigationManager.NavigateTo("/");
            return;
        }

        if (this.TeamId is not null && !this.CurrentParty.Teams.TryGetValue(this.TeamId.Value, out this.CurrentTeam!))
            this.NavigationManager.NavigateTo("/");

        this.CurrentParty.PartyChanged += async (_, _) => await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
    }
}
