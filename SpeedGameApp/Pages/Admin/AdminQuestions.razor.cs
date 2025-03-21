namespace SpeedGameApp.Pages.Admin;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

using SpeedGameApp.Business.Services;
using SpeedGameApp.DataAccessLayer;
using SpeedGameApp.DataAccessLayer.Entities;
using SpeedGameApp.Extensions;

public partial class AdminQuestions
{
    private List<QcmQuestion> questions = new();

    [Inject]
    public CsvService CsvService { get; set; } = default!;

    [Inject]
    public AppContext AppContext { get; set; } = default!;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
        => this.questions = await this.AppContext.Questions.Include(q => q.Theme).ToListAsync();

    private async Task InputFileHandlerAsync(InputFileChangeEventArgs arg)
    {
        var file = arg.File;
        var lines = await file.ConvertFileToStringArrayAsync();

        try
        {
            var questionsParsed = this.CsvService.CsvToQuestions(lines.Skip(1));
            await this.CsvService.InsertQuestionsAsync([.. questionsParsed]);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
