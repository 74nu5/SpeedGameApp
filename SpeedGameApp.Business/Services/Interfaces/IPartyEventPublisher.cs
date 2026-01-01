namespace SpeedGameApp.Business.Services.Interfaces;

using SpeedGameApp.Business.Data;

/// <summary>
///     Interface for publishing party events.
/// </summary>
public interface IPartyEventPublisher
{
    /// <summary>
    ///     Event raised when a party changes.
    /// </summary>
    event EventHandler<PartyDto>? PartyChanged;

    /// <summary>
    ///     Raises the party changed event.
    /// </summary>
    /// <param name="party">The party that changed.</param>
    void OnPartyChanged(PartyDto party);
}
