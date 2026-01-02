namespace SpeedGameApp.Pages;

using Microsoft.AspNetCore.Components;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services.Interfaces;

/// <summary>
///     The party page base.
/// </summary>
public class PartyPageBase : GamePageBase
{
    /// <inheritdoc />
    public PartyPageBase(IPartyManagementService partyManagementService, IQcmService qcmService, IGameplayService gameplayService, IThemeService themeService, NavigationManager navigationManager)
            : base(partyManagementService, qcmService, gameplayService, themeService, navigationManager)
    {
    }

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

        TeamDto? currentTeam = null;
        if (this.TeamId is not null && !this.CurrentParty.Teams.TryGetValue(this.TeamId.Value, out currentTeam))
            this.NavigationManager.NavigateTo("/");

        this.CurrentTeam = currentTeam!;

        this.CurrentParty.PartyChanged += async (_, _) => await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
    }
}
