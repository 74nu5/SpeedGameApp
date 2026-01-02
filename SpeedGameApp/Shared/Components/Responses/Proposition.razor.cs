namespace SpeedGameApp.Shared.Components.Responses;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

/// <summary>
///     Blazor component allowing the user to enter a response proposition.
/// </summary>
public partial class Proposition
{
    /// <summary>
    ///     Reference to the input field for the response.
    /// </summary>
    private InputText? inputResponse;

    /// <summary>
    ///     Stores the current response entered by the user.
    /// </summary>
    private string response = string.Empty;

    /// <summary>
    ///     Event callback triggered when the user submits a response.
    /// </summary>
    [Parameter]
    public EventCallback<string> ResponseCallback { get; set; }

    /// <summary>
    ///     Indicates whether a response has already been submitted.
    /// </summary>
    [Parameter]
    public bool AlreadyResponse { get; set; }

    /// <summary>
    ///     Resets the response input to an empty string and refreshes the component state.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    public async Task ResetAsync()
    {
        this.response = string.Empty;
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
    }

    /// <summary>
    ///     Sets focus to the input field after the component has rendered.
    /// </summary>
    /// <param name="firstRender">Indicates if this is the first render.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var inputResponseElement = this.inputResponse?.Element;
        if (inputResponseElement is not null)
            await inputResponseElement.Value.FocusAsync().ConfigureAwait(true);
    }

    /// <summary>
    ///     Invokes the <see cref="ResponseCallback" /> event with the current response.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    private async Task CallbackPropositionAsync()
        => await this.ResponseCallback.InvokeAsync(this.response).ConfigureAwait(true);
}
