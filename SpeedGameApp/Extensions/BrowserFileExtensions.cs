namespace SpeedGameApp.Extensions;

using Microsoft.AspNetCore.Components.Forms;

public static class BrowserFileExtensions
{
    public static async Task<string[]> ConvertFileToStringArrayAsync(this IBrowserFile file)
    {
        // Read the file content as a stream
        await using var stream = file.OpenReadStream();

        // Create a StreamReader to read the stream
        using var reader = new StreamReader(stream);

        // Read the file content line by line and store it in a list
        var lines = new List<string>();

        while (await reader.ReadLineAsync() is { } line)
        {
            lines.Add(line);
        }

        // Convert the list to an array and return it
        return lines.ToArray();
    }
}
