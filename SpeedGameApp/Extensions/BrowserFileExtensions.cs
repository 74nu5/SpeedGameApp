namespace SpeedGameApp.Extensions;

using Microsoft.AspNetCore.Components.Forms;

/// <summary>
///     Fournit des méthodes d'extension pour la manipulation des fichiers <see cref="IBrowserFile" /> dans Blazor.
/// </summary>
public static class BrowserFileExtensions
{
    /// <summary>
    ///     Lit le contenu d'un fichier <see cref="IBrowserFile" /> et le convertit en un tableau de chaînes, chaque élément représentant une ligne du fichier.
    /// </summary>
    /// <param name="file">Le fichier à lire.</param>
    /// <returns>
    ///     Une tâche asynchrone qui, lorsqu'elle est terminée, retourne un tableau de chaînes contenant chaque ligne du fichier.
    /// </returns>
    public static async Task<string[]> ConvertFileToStringArrayAsync(this IBrowserFile file)
    {
        // Lire le contenu du fichier en tant que flux
        await using var stream = file.OpenReadStream();

        // Créer un StreamReader pour lire le flux
        using var reader = new StreamReader(stream);

        // Lire le contenu du fichier ligne par ligne et le stocker dans une liste
        var lines = new List<string>();

        while (await reader.ReadLineAsync() is { } line)
            lines.Add(line);

        // Convert the list to an array and return it
        return lines.ToArray();
    }
}
