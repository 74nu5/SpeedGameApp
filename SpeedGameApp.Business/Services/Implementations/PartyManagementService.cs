namespace SpeedGameApp.Business.Services.Implementations;

using SpeedGameApp.Business.Common;
using SpeedGameApp.Business.Data;
using SpeedGameApp.Business.Services.Interfaces;
using SpeedGameApp.Business.Validators;
using SpeedGameApp.DataAccessLayer.Interfaces;

/// <summary>
///     Service responsible for party and team lifecycle management (CRUD operations).
/// </summary>
public sealed class PartyManagementService(
    IPartyRepository partyRepository,
    IPartyAccessLayer partyAccessLayer,
    IPartyEventPublisher eventPublisher,
    PartyNameValidator partyNameValidator,
    TeamNameValidator teamNameValidator) : IPartyManagementService
{
    /// <inheritdoc />
    public event EventHandler<PartyDto>? PartyChanged
    {
        add => eventPublisher.PartyChanged += value;
        remove => eventPublisher.PartyChanged -= value;
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<Guid, PartyDto> Parties
        => partyRepository.Parties;

    /// <inheritdoc />
    public async Task<Result<Guid>> CreatePartyAsync(string partyName, CancellationToken cancellationToken)
    {
        // Validate party name
        var validationResult = await partyNameValidator.ValidateAsync(partyName, cancellationToken);
        if (!validationResult.IsValid)
            return Result<Guid>.Failure(validationResult.Errors[0].ErrorMessage);

        // Create the party in the database
        var party = await partyAccessLayer.CreatePartyAsync(partyName, cancellationToken);

        // Add the party to the repository
        partyRepository.AddParty(party.Id, partyName);

        // Return the ID of the newly created party
        return Result<Guid>.Success(party.Id);
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> CreateTeamPartyAsync(Guid partyId, string teamName, CancellationToken cancellationToken)
    {
        // Validate team name
        var validationResult = await teamNameValidator.ValidateAsync(teamName, cancellationToken);
        if (!validationResult.IsValid)
            return Result<Guid>.Failure(validationResult.Errors[0].ErrorMessage);

        // Check if party exists
        if (!partyRepository.ExistsParty(partyId))
            return Result<Guid>.Failure("La partie spécifiée n'existe pas.");

        // Create team party
        var team = await partyAccessLayer.CreateTeamPartyAsync(partyId, teamName, cancellationToken);

        if (team is null)
            return Result<Guid>.Failure("Impossible de créer l'équipe.");

        // Add team to repository and return its ID
        _ = partyRepository.AddTeamParty(partyId, team);
        return Result<Guid>.Success(team.Id);
    }

    /// <inheritdoc />
    public async Task<PartyDto?> GetPartyAsync(Guid id, CancellationToken cancellationToken)
    {
        // Check if the party exists in the repository
        if (partyRepository.Parties.TryGetValue(id, out var party))
            return party;

        // If not, retrieve the party from the database
        var dbParty = await partyAccessLayer.GetPartyAsync(id, cancellationToken);

        // If the party was not found in the database, return null
        // Otherwise, load the party into the repository and return it
        return dbParty is null ? default : partyRepository.LoadParty(PartyDto.FromDbParty(dbParty));
    }

    /// <inheritdoc />
    public async Task<Dictionary<Guid, PartyDto>> GetDbPartiesAsync(CancellationToken cancellationToken)
    {
        // Retrieve the parties from the database
        var dbParties = await partyAccessLayer.GetPartiesAsync(cancellationToken);

        // Convert the database parties to DTOs
        return PartyDto.FromDbParties(dbParties);
    }

    /// <inheritdoc />
    public async Task LoadPartyAsync(Guid id, CancellationToken cancellationToken)
    {
        var partyFound = await partyAccessLayer.GetPartyAsync(id, cancellationToken);

        if (partyFound is null)
            return;

        _ = partyRepository.LoadParty(PartyDto.FromDbParty(partyFound));
    }

    /// <inheritdoc />
    public async Task SavePartyAsync(Guid id, CancellationToken cancellationToken)
    {
        // Check if the party exists in the repository
        if (!partyRepository.Parties.TryGetValue(id, out var party))
            return;

        // Loop through each team in the party and update their score
        foreach (var (key, team) in party.Teams)
            await partyAccessLayer.UpdateScoreAsync(key, team.Score, cancellationToken);
    }

    /// <inheritdoc />
    public void DeleteParty(Guid id)
        => partyRepository.DeleteParty(id);

    /// <inheritdoc />
    public void DeleteAllParties()
        => partyRepository.DeleteAllParties();

    /// <inheritdoc />
    public async Task DeleteDbPartyAsync(Guid id, CancellationToken cancellationToken)
        => await partyAccessLayer.DeletePartyAsync(id, cancellationToken);

    /// <inheritdoc />
    public async Task DeleteTeamAsync(Guid partyId, Guid teamId, CancellationToken cancellationToken)
    {
        await partyAccessLayer.DeleteTeamAsync(partyId, teamId, cancellationToken);
        partyRepository.RemoveTeam(partyId, teamId);
    }
}
