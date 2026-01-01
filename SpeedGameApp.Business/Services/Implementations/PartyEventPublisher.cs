namespace SpeedGameApp.Business.Services.Implementations;

using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services.Interfaces;

/// <summary>
///     Publishes party change events.
/// </summary>
internal sealed class PartyEventPublisher : IPartyEventPublisher
{
    /// <inheritdoc/>
    public event EventHandler<PartyDto>? PartyChanged;

    /// <inheritdoc/>
    public void OnPartyChanged(PartyDto party)
        => this.PartyChanged?.Invoke(this, party);
}
