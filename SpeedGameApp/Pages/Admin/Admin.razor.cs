namespace SpeedGameApp.Pages.Admin;

using SpeedGameApp.Business.Data;

public partial class Admin : GamePageBase
{
    private Dictionary<Guid, PartyDto> dbParties = new();

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        this.PartyManagementService.PartyChanged += async (_, _) => await this.InvokeAsync(this.StateHasChanged);
        this.dbParties = await this.PartyManagementService.GetDbPartiesAsync(this.CancellationTokenSource.Token);
    }

    private void DeleteParty(Guid id) => this.PartyManagementService.DeleteParty(id);

    private void DeleteAllParties() => this.PartyManagementService.DeleteAllParties();

    private async Task SavePartyAsync(Guid id)
    {
        this.CancellationTokenSource = new();
        await this.PartyManagementService.SavePartyAsync(id, this.CancellationTokenSource.Token);
        this.dbParties = await this.PartyManagementService.GetDbPartiesAsync(this.CancellationTokenSource.Token);
        await this.InvokeAsync(this.StateHasChanged);
    }

    private async Task DeleteDbPartyAsync(Guid id)
    {
        this.CancellationTokenSource = new();
        await this.PartyManagementService.DeleteDbPartyAsync(id, this.CancellationTokenSource.Token);
        this.dbParties = await this.PartyManagementService.GetDbPartiesAsync(this.CancellationTokenSource.Token);
        await this.InvokeAsync(this.StateHasChanged);
    }

    private async Task LoadPartyAsync(Guid id)
    {
        this.CancellationTokenSource = new();
        await this.PartyManagementService.LoadPartyAsync(id, this.CancellationTokenSource.Token);
        await this.InvokeAsync(this.StateHasChanged);
    }
}
