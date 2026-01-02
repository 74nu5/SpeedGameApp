namespace SpeedGameApp.Pages.Admin;

using Microsoft.AspNetCore.Components;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services.Interfaces;

public partial class Admin : GamePageBase
{
    private Dictionary<Guid, PartyDto> dbParties = new();
    private bool showDeleteDialog = false;
    private bool showDeleteAllDialog = false;
    private string deleteDialogMessage = string.Empty;
    private Guid? partyToDeleteId = null;
    private bool isDbParty = false;

    /// <inheritdoc />
    public Admin(IPartyManagementService partyManagementService, IQcmService qcmService, IGameplayService gameplayService, IThemeService themeService, NavigationManager navigationManager)
            : base(partyManagementService, qcmService, gameplayService, themeService, navigationManager)
    {
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        this.PartyManagementService.PartyChanged += async (_, _) => await this.InvokeAsync(this.StateHasChanged);
        this.dbParties = await this.PartyManagementService.GetDbPartiesAsync(this.CancellationTokenSource.Token);
    }

    private void ShowDeleteDialog(Guid id, string partyName, bool fromDatabase)
    {
        this.partyToDeleteId = id;
        this.isDbParty = fromDatabase;
        this.deleteDialogMessage = $"Voulez-vous vraiment supprimer la partie '{partyName}' ? Cette action est irréversible.";
        this.showDeleteDialog = true;
    }

    private async Task ConfirmDelete()
    {
        if (this.partyToDeleteId.HasValue)
        {
            if (this.isDbParty)
            {
                await this.DeleteDbPartyAsync(this.partyToDeleteId.Value);
            }
            else
            {
                this.DeleteParty(this.partyToDeleteId.Value);
            }

            this.partyToDeleteId = null;
        }

        this.showDeleteDialog = false;
    }

    private void CancelDelete()
    {
        this.partyToDeleteId = null;
        this.showDeleteDialog = false;
    }

    private void ShowDeleteAllDialog()
    {
        this.showDeleteAllDialog = true;
    }

    private void ConfirmDeleteAll()
    {
        this.DeleteAllParties();
        this.showDeleteAllDialog = false;
    }

    private void CancelDeleteAll()
    {
        this.showDeleteAllDialog = false;
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
