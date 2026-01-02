namespace SpeedGameApp.Shared.Components.Responses;

using Microsoft.AspNetCore.Components;

using SpeedGameApp.Business.Services.Models;

/// <summary>
///     Represents a Blazor component for displaying and handling a multiple-choice question (QCM).
/// </summary>
public partial class QCM
{
    /// <summary>
    ///     Stores the user's selected response.
    /// </summary>
    private string response = string.Empty;

    /// <summary>
    ///     Gets or sets the question data to display.
    /// </summary>
    [Parameter]
    public QcmQuestionDto? Question { get; set; }

    /// <summary>
    ///     Gets or sets the callback invoked when the user submits a response.
    /// </summary>
    [Parameter]
    public EventCallback<string> ResponseCallback { get; set; }

    /// <summary>
    ///     Resets the user's response and updates the component state.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    public async Task ResetAsync()
    {
        this.response = string.Empty;
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
    }

    /// <summary>
    ///     Invokes the <see cref="ResponseCallback" /> with the current response.
    /// </summary>
    private async Task CallbackQcmAsync()
        => await this.ResponseCallback.InvokeAsync(this.response).ConfigureAwait(true);
}
