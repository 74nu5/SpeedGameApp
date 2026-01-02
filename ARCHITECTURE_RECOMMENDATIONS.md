# Recommandations d'Architecture et Bonnes Pratiques

Document de recommandations pour améliorer l'architecture, la maintenabilité et la qualité du projet SpeedGameApp.

**Date** : 2 janvier 2026
**Contexte** : Projet personnel, pas de production publique

---

## 📋 Table des matières

1. [Recommandations Priorité HAUTE](#haute-priorité)
2. [Recommandations Priorité MOYENNE](#moyenne-priorité)
3. [Recommandations Priorité BASSE](#basse-priorité)
4. [Recommandations Optionnelles](#optionnel)
5. [Anti-Patterns à Éviter](#anti-patterns)

---

## 🔴 HAUTE PRIORITÉ

Ces recommandations amélioreront significativement la qualité et la maintenabilité du code.

### 1. **Ajouter des Interfaces pour les Access Layers**

**Problème** : Les access layers sont des classes concrètes sans interfaces.

**Impact** :
- ❌ Impossible de mocker pour les tests
- ❌ Couplage fort avec les implémentations
- ❌ Viole le Dependency Inversion Principle

**Solution** :

```csharp
// Créer les interfaces
public interface IPartyAccessLayer
{
    Task<Party> CreatePartyAsync(string partyName, CancellationToken cancellationToken);
    Task<Team?> CreateTeamPartyAsync(Guid partyId, string? teamName, CancellationToken cancellationToken);
    Task<Party?> GetPartyAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Party>> GetPartiesAsync(CancellationToken cancellationToken);
    Task DeletePartyAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateScoreAsync(Guid key, int teamScore, CancellationToken cancellationToken);
    Task DeleteTeamAsync(Guid partyId, Guid teamId, CancellationToken cancellationToken);
}

public interface IThemeAccessLayer
{
    Task<List<Theme>> GetAllThemesAsync();
}

public interface IQuestionAccessLayer
{
    QcmQuestion GetRandom();
}

// Implémenter
public sealed class PartyAccessLayer(AppContext context) : IPartyAccessLayer
{
    // ... implementation
}

// Enregistrer dans le DI
services.TryAddScoped<IPartyAccessLayer, PartyAccessLayer>();
services.TryAddScoped<IThemeAccessLayer, ThemeAccessLayer>();
services.TryAddScoped<IQuestionAccessLayer, QuestionAccessLayer>();

// Injecter les interfaces dans GameService
public sealed class GameService(
    IPartyRepository partyRepository,
    IPartyStateManager stateManager,
    IPartyEventPublisher eventPublisher,
    IThemeManager themeManager,
    IPartyAccessLayer partyAccessLayer,  // ✅ Interface
    IQuestionAccessLayer questionAccessLayer,  // ✅ Interface
    IThemeAccessLayer themeAccessLayer,  // ✅ Interface
    TimeProvider timeProvider)
```

**Bénéfices** :
- ✅ Testabilité (mocking facile)
- ✅ Respect du DIP
- ✅ Changement d'implémentation facile (ex: passer à Dapper)

---

### 2. **Implémenter un Pattern Result<T> pour la Gestion d'Erreurs**

**Problème** : Les méthodes retournent `null` ou des valeurs par défaut en cas d'erreur.

**Code actuel** :
```csharp
public async Task<Guid?> CreateTeamPartyAsync(Guid partyId, string? teamName, CancellationToken cancellationToken)
{
    if (string.IsNullOrWhiteSpace(teamName))
        return default;  // ❌ Pourquoi null? Erreur de validation?

    if (!partyRepository.ExistsParty(partyId))
        return default;  // ❌ Pourquoi null? Party pas trouvée?

    // ...
}
```

**Solution** : Utiliser un pattern Result<T>

```csharp
// Créer une classe Result
public readonly record struct Result<T>
{
    public T? Value { get; init; }
    public bool IsSuccess { get; init; }
    public string Error { get; init; }

    public static Result<T> Success(T value) => new() { Value = value, IsSuccess = true };
    public static Result<T> Failure(string error) => new() { Error = error, IsSuccess = false };
}

// Utiliser dans les méthodes
public async Task<Result<Guid>> CreateTeamPartyAsync(Guid partyId, string? teamName, CancellationToken cancellationToken)
{
    if (string.IsNullOrWhiteSpace(teamName))
        return Result<Guid>.Failure("Team name cannot be empty");

    if (!partyRepository.ExistsParty(partyId))
        return Result<Guid>.Failure($"Party {partyId} not found");

    var team = await partyAccessLayer.CreateTeamPartyAsync(partyId, teamName, cancellationToken);

    if (team is null)
        return Result<Guid>.Failure("Failed to create team in database");

    _ = partyRepository.AddTeamParty(partyId, team);
    return Result<Guid>.Success(team.Id);
}

// Dans l'appelant (Razor)
var result = await GameService.CreateTeamPartyAsync(partyId, teamName, cancellationToken);
if (!result.IsSuccess)
{
    ShowError(result.Error);  // Message clair à l'utilisateur
    return;
}

var teamId = result.Value;
```

**Alternative** : Utiliser une bibliothèque comme **FluentResults** ou **LanguageExt**.

**Bénéfices** :
- ✅ Gestion d'erreurs explicite
- ✅ Messages d'erreur clairs
- ✅ Pas de null references
- ✅ Meilleure expérience utilisateur

---

### 3. **Ajouter de la Validation avec FluentValidation**

**Problème** : Aucune validation des inputs.

**Solution** :

```csharp
// Install-Package FluentValidation.DependencyInjectionExtensions

// Créer des validateurs
public class CreatePartyCommandValidator : AbstractValidator<string>
{
    public CreatePartyCommandValidator()
    {
        RuleFor(name => name)
            .NotEmpty().WithMessage("Le nom de la partie est requis")
            .MinimumLength(3).WithMessage("Le nom doit contenir au moins 3 caractères")
            .MaximumLength(50).WithMessage("Le nom ne peut pas dépasser 50 caractères");
    }
}

public class CreateTeamCommandValidator : AbstractValidator<(Guid PartyId, string TeamName)>
{
    public CreateTeamCommandValidator()
    {
        RuleFor(x => x.PartyId)
            .NotEmpty().WithMessage("L'ID de la partie est requis");

        RuleFor(x => x.TeamName)
            .NotEmpty().WithMessage("Le nom de l'équipe est requis")
            .MinimumLength(2).WithMessage("Le nom doit contenir au moins 2 caractères")
            .MaximumLength(30).WithMessage("Le nom ne peut pas dépasser 30 caractères");
    }
}

// Enregistrer dans le DI
services.AddValidatorsFromAssemblyContaining<CreatePartyCommandValidator>();

// Utiliser dans GameService
public sealed class GameService(
    IPartyRepository partyRepository,
    IValidator<string> partyNameValidator,
    IValidator<(Guid, string)> teamValidator,
    // ... autres dépendances
)
{
    public async Task<Result<Guid>> CreatePartyAsync(string partyName, CancellationToken cancellationToken)
    {
        var validationResult = await partyNameValidator.ValidateAsync(partyName, cancellationToken);
        if (!validationResult.IsValid)
            return Result<Guid>.Failure(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var party = await partyAccessLayer.CreatePartyAsync(partyName, cancellationToken);
        partyRepository.AddParty(party.Id, partyName);
        return Result<Guid>.Success(party.Id);
    }
}
```

**Bénéfices** :
- ✅ Validation centralisée et réutilisable
- ✅ Messages d'erreur clairs et localisables
- ✅ Logique de validation testable
- ✅ Séparation des responsabilités

---

### 4. **Corriger les Requêtes Synchrones dans les Access Layers**

**Problème** : Utilisation de `FirstOrDefault` au lieu de `FirstOrDefaultAsync`.

**Code problématique** :
```csharp
// PartyAccessLayer.cs ligne 22
var party = context.Parties.FirstOrDefault(p => p.Id == partyId);  // ❌ Bloque le thread
```

**Solution** :
```csharp
var party = await context.Parties.FirstOrDefaultAsync(p => p.Id == partyId, cancellationToken);  // ✅ Async
```

**Bénéfices** :
- ✅ Ne bloque pas les threads
- ✅ Meilleure scalabilité
- ✅ Support du cancellation

---

### 5. **Renommer AppContext en SpeedGameDbContext**

**Problème** : `AppContext` est un nom trop générique qui peut créer de la confusion.

**Solution** :
```csharp
// Avant
public sealed class AppContext : DbContext { }

// Après
public sealed class SpeedGameDbContext : DbContext { }
```

**Bénéfices** :
- ✅ Nom explicite et clair
- ✅ Évite les conflits de nommage
- ✅ Meilleure lisibilité

---

## 🟡 MOYENNE PRIORITÉ

Améliorations qui augmenteront la qualité du code sans être urgentes.

### 6. **Implémenter le Pattern Unit of Work**

**Problème** : `SaveChangesAsync` appelé directement partout.

**Solution** :
```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

public sealed class UnitOfWork(SpeedGameDbContext context) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        => _transaction = await context.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}

// Utiliser dans les opérations complexes
public async Task<Result<Guid>> CreatePartyWithTeamsAsync(
    string partyName,
    List<string> teamNames,
    CancellationToken cancellationToken)
{
    await unitOfWork.BeginTransactionAsync(cancellationToken);

    try
    {
        var partyResult = await CreatePartyAsync(partyName, cancellationToken);
        if (!partyResult.IsSuccess)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<Guid>.Failure(partyResult.Error);
        }

        foreach (var teamName in teamNames)
        {
            var teamResult = await CreateTeamPartyAsync(partyResult.Value, teamName, cancellationToken);
            if (!teamResult.IsSuccess)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<Guid>.Failure(teamResult.Error);
            }
        }

        await unitOfWork.CommitTransactionAsync(cancellationToken);
        return partyResult;
    }
    catch
    {
        await unitOfWork.RollbackTransactionAsync(cancellationToken);
        throw;
    }
}
```

---

### 7. **Séparer PartyDto en DTO et Entité de Domaine**

**Problème** : `PartyDto` a trop de responsabilités :
- DTO (transfert de données)
- Logique de mapping (`FromDbParty`)
- Événements (`PartyChanged`, `PartyReset`)
- État de jeu (`AlreadyResponse`, `CurrentQcm`)

**Solution** : Séparer en plusieurs classes

```csharp
// 1. DTO pur (transfert de données)
public sealed record PartyDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required IReadOnlyList<TeamDto> Teams { get; init; }
}

// 2. Entité de domaine (logique métier)
public sealed class Party
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    private readonly List<Team> teams = [];
    public IReadOnlyList<Team> Teams => teams.AsReadOnly();

    public event EventHandler? PartyChanged;
    public event EventHandler? PartyReset;

    private Party(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public static Party Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        return new Party(Guid.NewGuid(), name);
    }

    public void AddTeam(Team team)
    {
        if (team is null)
            throw new ArgumentNullException(nameof(team));

        teams.Add(team);
        OnPartyChanged();
    }

    private void OnPartyChanged() => PartyChanged?.Invoke(this, EventArgs.Empty);
}

// 3. Mapper séparé (AutoMapper ou manuel)
public static class PartyMapper
{
    public static PartyDto ToDto(Party party) => new()
    {
        Id = party.Id,
        Name = party.Name,
        Teams = party.Teams.Select(TeamMapper.ToDto).ToList()
    };

    public static Party ToDomain(DataAccessLayer.Entities.Party dbParty) => // ...
}
```

---

### 8. **Ajouter du Logging Structuré avec Serilog**

**Problème** : Aucun logging dans l'application.

**Solution** :
```csharp
// Install-Package Serilog.AspNetCore
// Install-Package Serilog.Sinks.Console
// Install-Package Serilog.Sinks.File

// Program.cs
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/speedgame-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Dans les services
public sealed class GameService(
    ILogger<GameService> logger,  // Injecté automatiquement
    // ... autres dépendances
)
{
    public async Task<Result<Guid>> CreatePartyAsync(string partyName, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating party with name {PartyName}", partyName);

        try
        {
            var party = await partyAccessLayer.CreatePartyAsync(partyName, cancellationToken);
            logger.LogInformation("Party {PartyId} created successfully", party.Id);
            return Result<Guid>.Success(party.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create party with name {PartyName}", partyName);
            return Result<Guid>.Failure("Failed to create party");
        }
    }
}
```

**Bénéfices** :
- ✅ Debugging facilité
- ✅ Audit trail
- ✅ Monitoring en production

---

### 9. **Centraliser la Configuration**

**Problème** : Magic numbers en dur dans le code.

**Code actuel** :
```csharp
public void GenerateThemes(Guid partyId)
{
    // ...
    for (var i = 0; i < 5; i++)  // ❌ Magic number
    {
        themes.Add(new() { Id = Guid.NewGuid(), Name = themeTeam.Name, Team = team });
    }

    themes.AddRange(themes.Count < 50 ? otherThemesLimited.Take(50 - themes.Count) : otherThemesLimited);  // ❌ Magic number
}
```

**Solution** :
```csharp
// appsettings.json
{
  "GameSettings": {
    "ThemeSettings": {
      "TotalThemesPerGame": 50,
      "ThemeRepetitionCount": 5
    },
    "ScoringSettings": {
      "CorrectAnswerPoints": 100,
      "IncorrectAnswerPenalty": -10
    }
  }
}

// Configuration class
public sealed class ThemeSettings
{
    public int TotalThemesPerGame { get; set; } = 50;
    public int ThemeRepetitionCount { get; set; } = 5;
}

public sealed class GameSettings
{
    public ThemeSettings ThemeSettings { get; set; } = new();
    public ScoringSettings ScoringSettings { get; set; } = new();
}

// Program.cs
builder.Services.Configure<GameSettings>(builder.Configuration.GetSection("GameSettings"));

// Utiliser
public sealed class GameService(
    IOptions<GameSettings> settings,
    // ... autres dépendances
)
{
    private readonly GameSettings gameSettings = settings.Value;

    public void GenerateThemes(Guid partyId)
    {
        var themesCount = gameSettings.ThemeSettings.TotalThemesPerGame;  // ✅ Configurable
        var repetitionCount = gameSettings.ThemeSettings.ThemeRepetitionCount;  // ✅ Configurable

        // ...
    }
}
```

---

### 10. **Utiliser AutoMapper pour le Mapping**

**Problème** : Mapping manuel partout.

**Solution** :
```csharp
// Install-Package AutoMapper.Extensions.Microsoft.DependencyInjection

// Créer des profiles
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<DataAccessLayer.Entities.Party, PartyDto>()
            .ForMember(dest => dest.Teams, opt => opt.MapFrom(src => src.Teams));

        CreateMap<DataAccessLayer.Entities.Team, TeamDto>();
    }
}

// Enregistrer
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Utiliser
public sealed class GameService(
    IMapper mapper,
    // ... autres dépendances
)
{
    public async Task<PartyDto?> GetPartyAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbParty = await partyAccessLayer.GetPartyAsync(id, cancellationToken);
        return dbParty is null ? null : mapper.Map<PartyDto>(dbParty);  // ✅ Automatique
    }
}
```

---

## 🟢 BASSE PRIORITÉ

Nice-to-have qui amélioreront l'expérience développeur.

### 11. **Implémenter des Health Checks**

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContextCheck<SpeedGameDbContext>()
    .AddCheck<PartyRepositoryHealthCheck>("party-repository");

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
```

### 12. **Ajouter Swagger/OpenAPI pour l'API**

```csharp
// Déjà installé mais pas configuré
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

### 13. **Utiliser Records Immuables pour les DTOs**

**Actuel** :
```csharp
public sealed record PartyDto(Guid Id, string Name, Dictionary<Guid, TeamDto> Teams)
{
    public ResponseType CurrentResponseType { get; internal set; }  // ❌ Mutable
    public bool ShowThemes { get; set; }  // ❌ Mutable
}
```

**Recommandé** :
```csharp
public sealed record PartyDto(
    Guid Id,
    string Name,
    IReadOnlyDictionary<Guid, TeamDto> Teams,
    ResponseType CurrentResponseType,
    bool ShowThemes,
    bool AlreadyResponse,
    QcmQuestionDto? CurrentQcm)
{
    // Méthode With pour créer des copies modifiées
    public PartyDto WithResponseType(ResponseType responseType)
        => this with { CurrentResponseType = responseType };
}
```

### 14. **Ajouter des Migrations EF Core**

**Actuel** : Pas de migrations (probablement EnsureCreated)

**Recommandé** :
```bash
# Créer la migration initiale
dotnet ef migrations add InitialCreate --project SpeedGameApp.DataAccessLayer

# Appliquer en startup
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<SpeedGameDbContext>();
    await context.Database.MigrateAsync();
}
```

---

## ⚫ OPTIONNEL

Pour aller encore plus loin (peut-être overkill pour un projet perso).

### 15. **Implémenter CQRS avec MediatR**

```csharp
// Install-Package MediatR

// Commands
public record CreatePartyCommand(string Name) : IRequest<Result<Guid>>;

public sealed class CreatePartyCommandHandler(
    IPartyAccessLayer partyAccessLayer,
    IPartyRepository partyRepository)
    : IRequestHandler<CreatePartyCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePartyCommand request, CancellationToken cancellationToken)
    {
        var party = await partyAccessLayer.CreatePartyAsync(request.Name, cancellationToken);
        partyRepository.AddParty(party.Id, request.Name);
        return Result<Guid>.Success(party.Id);
    }
}

// Queries
public record GetPartyQuery(Guid Id) : IRequest<PartyDto?>;

public sealed class GetPartyQueryHandler(
    IPartyRepository repository)
    : IRequestHandler<GetPartyQuery, PartyDto?>
{
    public Task<PartyDto?> Handle(GetPartyQuery request, CancellationToken cancellationToken)
        => Task.FromResult(repository.GetParty(request.Id));
}

// Utiliser
public sealed class PartyController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateParty([FromBody] CreatePartyCommand command)
    {
        var result = await mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
```

### 16. **Domain Events**

```csharp
public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
}

public record PartyCreatedEvent(Guid PartyId, string PartyName) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task Handle(TEvent @event, CancellationToken cancellationToken);
}
```

### 17. **Specification Pattern pour les Requêtes**

```csharp
public abstract class Specification<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();
    public bool IsSatisfiedBy(T entity) => ToExpression().Compile()(entity);
}

public class ActivePartiesSpecification : Specification<Party>
{
    public override Expression<Func<Party, bool>> ToExpression()
        => party => party.Teams.Any();
}
```

---

## ❌ ANTI-PATTERNS À ÉVITER

### ❌ 1. God Service
Ne pas tout mettre dans GameService. Si une classe dépasse 300-400 lignes, c'est un red flag.

### ❌ 2. Anemic Domain Model
Les entités de domaine ne doivent pas être juste des sacs de propriétés. Ajoutez de la logique métier dedans.

### ❌ 3. Repository qui retourne des IQueryable
Les repositories doivent retourner des entités ou collections, pas des IQueryable.

### ❌ 4. Logique Métier dans les Controllers/Pages
Toute la logique doit être dans les services, pas dans les Razor pages.

### ❌ 5. Async Void
Toujours retourner `Task` ou `Task<T>`, jamais `async void` (sauf event handlers).

---

## 📊 Résumé des Priorités

| Priorité | Recommandation | Impact | Effort | ROI |
|----------|---------------|--------|--------|-----|
| 🔴 Haute | Interfaces Access Layers | +++  | Moyen | ⭐⭐⭐⭐⭐ |
| 🔴 Haute | Pattern Result<T> | +++ | Moyen | ⭐⭐⭐⭐⭐ |
| 🔴 Haute | FluentValidation | +++ | Moyen | ⭐⭐⭐⭐ |
| 🔴 Haute | Async Queries | ++ | Faible | ⭐⭐⭐⭐⭐ |
| 🔴 Haute | Renommer AppContext | + | Faible | ⭐⭐⭐ |
| 🟡 Moyenne | Unit of Work | ++ | Moyen | ⭐⭐⭐ |
| 🟡 Moyenne | Séparer DTO/Domain | +++ | Élevé | ⭐⭐⭐ |
| 🟡 Moyenne | Logging (Serilog) | ++ | Faible | ⭐⭐⭐⭐ |
| 🟡 Moyenne | Configuration centralisée | + | Faible | ⭐⭐⭐ |
| 🟡 Moyenne | AutoMapper | + | Faible | ⭐⭐ |
| 🟢 Basse | Health Checks | + | Faible | ⭐⭐ |
| 🟢 Basse | Swagger | + | Faible | ⭐⭐ |
| 🟢 Basse | Records immuables | + | Moyen | ⭐⭐ |
| 🟢 Basse | EF Migrations | ++ | Faible | ⭐⭐⭐ |
| ⚫ Optionnel | CQRS + MediatR | +++ | Élevé | ⭐ |
| ⚫ Optionnel | Domain Events | ++ | Élevé | ⭐ |
| ⚫ Optionnel | Specification Pattern | + | Moyen | ⭐ |

---

## 🎯 Plan d'Action Recommandé

### Phase 1 : Fondations (1-2 jours)
1. ✅ Créer interfaces pour Access Layers
2. ✅ Corriger les requêtes synchrones
3. ✅ Renommer AppContext

### Phase 2 : Qualité (2-3 jours)
4. ✅ Implémenter Result<T>
5. ✅ Ajouter FluentValidation
6. ✅ Ajouter Serilog

### Phase 3 : Architecture (3-4 jours)
7. ✅ Implémenter Unit of Work
8. ✅ Séparer DTO/Domain Model
9. ✅ Ajouter AutoMapper
10. ✅ Centraliser configuration

### Phase 4 : Polish (1-2 jours)
11. ✅ Health Checks
12. ✅ EF Migrations
13. ✅ Documentation

---

## 📚 Ressources

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Result Pattern](https://enterprisecraftsmanship.com/posts/error-handling-exception-or-result/)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [AutoMapper](https://docs.automapper.org/)
- [Serilog](https://serilog.net/)
- [MediatR](https://github.com/jbogard/MediatR)

---

**Document créé par Claude Code - 2 janvier 2026**

Ces recommandations sont adaptées à un projet personnel. Choisissez celles qui vous apportent le plus de valeur sans over-engineering !
