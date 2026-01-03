namespace SpeedGameApp.Shared;

using Microsoft.AspNetCore.Components;
using SpeedGameApp.Business.Services.Interfaces;
using SpeedGameApp.DataEnum;

/// <summary>
///     Base class for PlayerLayout that retrieves party theme.
/// </summary>
public class PlayerLayoutBase : LayoutComponentBase
{
    /// <summary>
    ///     Gets or sets the party repository service.
    /// </summary>
    [Inject]
    protected IPartyRepository PartyRepository { get; set; } = default!;

    /// <summary>
    ///     Gets or sets the navigation manager.
    /// </summary>
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    /// <summary>
    ///     Gets the current theme based on the party in context.
    /// </summary>
    protected PartyTheme CurrentTheme { get; private set; } = PartyTheme.ThreeBStudio;

    /// <summary>
    ///     Called when parameters are set.
    /// </summary>
    protected override void OnParametersSet()
    {
        // Try to extract party ID from URL and get its theme
        var uri = new Uri(this.NavigationManager.Uri);
        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        // Look for party/{guid} pattern in URL
        for (int i = 0; i < segments.Length - 1; i++)
        {
            if (segments[i].Equals("party", StringComparison.OrdinalIgnoreCase) &&
                Guid.TryParse(segments[i + 1], out Guid partyId))
            {
                var party = this.PartyRepository.GetParty(partyId);
                if (party != null)
                {
                    this.CurrentTheme = party.Theme;
                    return;
                }
            }
        }

        // Default theme if no party found
        this.CurrentTheme = PartyTheme.ThreeBStudio;
    }
}
