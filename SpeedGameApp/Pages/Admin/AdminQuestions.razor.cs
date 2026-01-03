namespace SpeedGameApp.Pages.Admin;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;

using SpeedGameApp.Business.Services;
using SpeedGameApp.DataAccessLayer;
using SpeedGameApp.DataAccessLayer.Entities;
using SpeedGameApp.DataEnum;
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

    private bool isDragging = false;
    private bool isUploading = false;
    private string uploadMessage = string.Empty;
    private bool uploadSuccess = false;

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
        this.isUploading = true;
        this.uploadMessage = string.Empty;
        this.StateHasChanged();

        var file = arg.File;

        try
        {
            var lines = await file.ConvertFileToStringArrayAsync();
            var questionsParsed = this.csvService.CsvToQuestions(lines.Skip(1));
            var questionsList = questionsParsed.ToList();

            await this.csvService.InsertQuestionsAsync(questionsList);

            this.uploadSuccess = true;
            this.uploadMessage = $"{questionsList.Count} question(s) importée(s) avec succès !";

            // Reload questions
            this.questions = await this.appContext.Questions.Include(q => q.Theme).ToListAsync();
        }
        catch (Exception e)
        {
            this.uploadSuccess = false;
            this.uploadMessage = $"Erreur lors de l'import : {e.Message}";
            Console.WriteLine(e);
        }
        finally
        {
            this.isUploading = false;
            this.StateHasChanged();
        }
    }

    private void HandleDragEnter(DragEventArgs e)
    {
        this.isDragging = true;
    }

    private void HandleDragLeave(DragEventArgs e)
    {
        this.isDragging = false;
    }

    private async Task HandleDrop(DragEventArgs e)
    {
        this.isDragging = false;

        // Note: File drop handling in Blazor requires InputFile component
        // The actual file handling is done through the InputFile OnChange event
        // This method is here for visual feedback only
        this.StateHasChanged();
    }

    private string GetDragDropClasses()
    {
        if (this.isDragging)
        {
            return "block border-4 border-dashed border-3b-blue bg-3b-blue/5 rounded-lg shadow-large";
        }

        if (this.isUploading)
        {
            return "block border-4 border-dashed border-3b-blue bg-3b-blue/5 rounded-lg animate-pulse";
        }

        return "block border-4 border-dashed border-gray-300 bg-gray-50 rounded-lg hover:border-3b-blue hover:bg-3b-blue/5 hover:shadow-medium";
    }

    private string GetDifficultyBadgeClass(Difficulty difficulty)
        => difficulty switch
        {
            Difficulty.Facile => "bg-green-100 text-green-800",
            Difficulty.Moyenne => "bg-yellow-100 text-yellow-800",
            Difficulty.Difficile => "bg-red-100 text-red-800",
            _ => "bg-gray-100 text-gray-800",
        };

    private string GetUploadMessageClasses()
    {
        if (this.uploadSuccess)
        {
            return "bg-green-100 border-green-500 text-green-700";
        }

        return "bg-red-100 border-red-500 text-red-700";
    }

    private string GetUploadMessageIcon()
    {
        if (this.uploadSuccess)
        {
            return "bi bi-check-circle-fill text-green-600";
        }

        return "bi bi-exclamation-triangle-fill text-red-600";
    }

    private string GetUploadMessageTitle()
    {
        if (this.uploadSuccess)
        {
            return "Succès";
        }

        return "Erreur";
    }

    private void ClearUploadMessage()
    {
        this.uploadMessage = string.Empty;
    }
}
