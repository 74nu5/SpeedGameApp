namespace SpeedGameApp.Shared.Components.Responses;

using Microsoft.AspNetCore.Components;

/// <summary>
///     Represents a buzzer component for user interaction in a game context.
/// </summary>
public sealed partial class Buzzer
{
    /// <summary>
    ///     Reference to the button element in the component.
    /// </summary>
    private ElementReference button;

    /// <summary>
    ///     Event callback triggered when the buzzer is pressed.
    /// </summary>
    [Parameter]
    public EventCallback Buzz { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether a response has already been given.
    /// </summary>
    [Parameter]
    public bool AlreadyResponse { get; set; }

    /// <summary>
    ///     Sets focus to the button element after the component has rendered.
    /// </summary>
    /// <param name="firstRender">Indicates whether this is the first time the component has rendered.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
        => await this.button.FocusAsync().ConfigureAwait(true);

    /// <summary>
    ///     Invokes the <see cref="Buzz" /> event callback asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    private async Task CallbackBuzzerAsync()
        => await this.Buzz.InvokeAsync().ConfigureAwait(true);
}
