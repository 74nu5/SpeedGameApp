namespace SpeedGameApp.Shared.Components;

using Microsoft.AspNetCore.Components;

public partial class Timer
{
    private TimeSpan time;

    [Parameter]
    public TimeSpan? Time { get; set; }

    public async void AdvanceTime()
    {
        this.time = this.time.Subtract(TimeSpan.FromSeconds(1));
        await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        var timeSpan = this.Time;

        if (timeSpan is not null)
            this.time = timeSpan.Value;
    }
}
