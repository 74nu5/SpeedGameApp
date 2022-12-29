namespace SpeedGameApp.Business.Data;

using SpeedGameApp.Business.Services.Models;
using SpeedGameApp.DataAccessLayer.Entities;
using SpeedGameApp.DataEnum;

/// <summary>
///     Record which represents a party.
/// </summary>
/// <param name="Id">The party id.</param>
/// <param name="Name">The party name.</param>
/// <param name="Teams">The party teams.</param>
public sealed record PartyDto(Guid Id, string Name, Dictionary<Guid, TeamDto> Teams)
{
    public static readonly PartyDto Empty = new(Guid.Empty, string.Empty);

    public PartyDto(Guid id, string name)
        : this(id, name, new())
    {
    }

    public event EventHandler? PartyChanged;

    public IEnumerable<TeamDto> TeamsSortedByScore => this.Teams.Select(kvp => kvp.Value).OrderByDescending(t => t.Score);

    public ResponseType CurrentResponseType { get; set; }

    public bool AlreadyResponse { get; set; }

    public QcmQuestionDto CurrentQcm { get; set; }

    public static PartyDto FromDbParty(Party dbParty)
        => new(dbParty.Id, dbParty.Name, TeamDto.FromDbTeams(dbParty.Id, dbParty.Teams));

    public static Dictionary<Guid, PartyDto> FromDbParties(IEnumerable<Party> dbParties)
        => dbParties.Select(FromDbParty).ToDictionary(p => p.Id, p => p);

    public void OnPartyChanged()
        => this.PartyChanged?.Invoke(this, EventArgs.Empty);
}
