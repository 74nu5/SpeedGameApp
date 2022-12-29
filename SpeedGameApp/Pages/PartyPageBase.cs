namespace SpeedGameApp.Pages;

using Microsoft.AspNetCore.Components;

using SpeedGameApp.Business.Data;

public class PartyPageBase : GamePageBase
{
    [Parameter]
    public Guid PartyId { get; set; }

    [Parameter]
    public Guid? TeamId { get; set; }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        this.CurrentParty = await this.GameService.GetPartyAsync(this.PartyId, this.CancellationTokenSource.Token) ?? PartyDto.Empty;

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
