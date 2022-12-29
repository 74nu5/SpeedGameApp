namespace SpeedGameApp.Pages.Admin;

using SpeedGameApp.Business.Data;

public partial class Admin : GamePageBase
{
    private Dictionary<Guid, PartyDto> dbParties = new();

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        this.GameService.PartyChanged += async (_, _) => await this.InvokeAsync(this.StateHasChanged);
        this.dbParties = await this.GameService.GetDbPartiesAsync(this.CancellationTokenSource.Token);
    }

    private void DeleteParty(Guid id) => this.GameService.DeleteParty(id);

    private void DeleteAllParties() => this.GameService.DeleteAllParties();

    private async Task SavePartyAsync(Guid id)
    {
        this.CancellationTokenSource = new();
        await this.GameService.SavePartyAsync(id, this.CancellationTokenSource.Token);
        this.dbParties = await this.GameService.GetDbPartiesAsync(this.CancellationTokenSource.Token);
        await this.InvokeAsync(this.StateHasChanged);
    }

    private async Task DeleteDbPartyAsync(Guid id)
    {
        this.CancellationTokenSource = new();
        await this.GameService.DeleteDbPartyAsync(id, this.CancellationTokenSource.Token);
        this.dbParties = await this.GameService.GetDbPartiesAsync(this.CancellationTokenSource.Token);
        await this.InvokeAsync(this.StateHasChanged);
    }

    private async Task LoadPartyAsync(Guid id)
    {
        this.CancellationTokenSource = new();
        await this.GameService.LoadPartyAsync(id, this.CancellationTokenSource.Token);
        await this.InvokeAsync(this.StateHasChanged);
    }
}
