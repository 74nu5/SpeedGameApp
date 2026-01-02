namespace SpeedGameApp.Pages.Admin;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

using SpeedGameApp.Business.Services;
using SpeedGameApp.DataAccessLayer;
using SpeedGameApp.DataAccessLayer.Entities;
using SpeedGameApp.Extensions;

/// <summary>
///     Composant Blazor pour la gestion des questions administrateur.
///     Permet l'affichage, l'importation et l'insertion de questions via un fichier CSV.
/// </summary>
public partial class AdminQuestions
{
    /// <summary>
    ///     Service pour la gestion des opérations CSV.
    /// </summary>
    private readonly CsvService csvService;

    /// <summary>
    ///     Contexte de base de données de l'application.
    /// </summary>
    private readonly SpeedGameDbContext appContext;

    /// <summary>
    ///     Liste des questions chargées depuis la base de données.
    /// </summary>
    private List<QcmQuestion> questions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminQuestions"/> class.
    ///     Initialise une nouvelle instance de la classe <see cref="AdminQuestions" />.
    /// </summary>
    /// <param name="csvService">Service de gestion des fichiers CSV.</param>
    /// <param name="appContext">Contexte de base de données de l'application.</param>
    public AdminQuestions(CsvService csvService, SpeedGameDbContext appContext)
    {
        this.csvService = csvService;
        this.appContext = appContext;
    }

    /// <summary>
    ///     Méthode appelée lors de l'initialisation du composant.
    ///     Charge la liste des questions avec leur thème associé depuis la base de données.
    /// </summary>
    /// <returns>Une tâche asynchrone représentant l'opération d'initialisation.</returns>
    protected override async Task OnInitializedAsync()
        => this.questions = await this.appContext.Questions.Include(q => q.Theme).ToListAsync();

    /// <summary>
    ///     Gère l'importation d'un fichier CSV contenant des questions.
    ///     Convertit le fichier en tableau de chaînes, parse les questions et les insère en base.
    /// </summary>
    /// <param name="arg">Événement de changement de fichier d'entrée.</param>
    /// <returns>Une tâche asynchrone représentant l'opération d'importation.</returns>
    private async Task InputFileHandlerAsync(InputFileChangeEventArgs arg)
    {
        var file = arg.File;
        var lines = await file.ConvertFileToStringArrayAsync();

        try
        {
            var questionsParsed = this.csvService.CsvToQuestions(lines.Skip(1));
            await this.csvService.InsertQuestionsAsync(questionsParsed.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
