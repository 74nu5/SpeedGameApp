namespace SpeedGameApp.Shared.Components.Responses;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

public sealed partial class TimedProposition
{
    private InputText? inputResponse;

    private string response = string.Empty;

    [Parameter]
    [EditorRequired]
    public EventCallback<string> ResponseCallback { get; set; }

    [Parameter]
    public bool Enabled { get; set; }

    public async Task ResetAsync()
    {
        this.response = string.Empty;
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var inputResponseElement = this.inputResponse?.Element;

        if (inputResponseElement is not null)
            await inputResponseElement.Value.FocusAsync().ConfigureAwait(true);
    }

    private async Task CallbackAsync()
        => await this.ResponseCallback.InvokeAsync(this.response).ConfigureAwait(true);
}
