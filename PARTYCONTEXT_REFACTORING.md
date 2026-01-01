# Refactoring PartyContext - Séparation des Responsabilités

Ce document explique le refactoring majeur du `PartyContext` selon le principe de responsabilité unique (Single Responsibility Principle - SRP).

## 📅 Date du refactoring
**1er janvier 2026** - Refactoring du PartyContext monolithique

---

## ❌ Problème identifié

Le `PartyContext` (257 lignes) violait le principe de responsabilité unique en gérant :

1. **Stockage des parties** - `ConcurrentDictionary<Guid, PartyDto>` en mémoire
2. **Gestion d'état du jeu** - Buzz, réponses, scores, QCM
3. **Publication d'événements** - `PartyChanged` event
4. **Gestion des thèmes** - Chargement, sélection, réinitialisation

C'était un **god object** : une classe qui fait trop de choses différentes, difficile à tester et à maintenir.

---

## ✅ Solution appliquée

Séparation en **4 services distincts avec interfaces** :

### 1. **IPartyRepository** - Stockage et récupération
```csharp
public interface IPartyRepository
{
    IReadOnlyDictionary<Guid, PartyDto> Parties { get; }
    PartyDto LoadParty(PartyDto partyDto);
    void AddParty(Guid partyId, string partyName);
    Guid? AddTeamParty(Guid partyId, Team team);
    void RemoveTeam(Guid partyId, Guid teamId);
    void DeleteParty(Guid partyId);
    void DeleteAllParties();
    bool ExistsParty(Guid partyId);
    PartyDto? GetParty(Guid partyId);
}
```

**Responsabilité** : CRUD des parties en mémoire

### 2. **IPartyStateManager** - Gestion d'état du jeu
```csharp
public interface IPartyStateManager
{
    void AddPoints(TeamDto teamDto, int points);
    void SetCurrentResponse(Guid partyId, ResponseType responseType);
    void BuzzTeam(Guid partyId, Guid teamId);
    void ResetTeam(Guid partyId);
    void PropositionTeam(Guid partyId, Guid teamId, string response);
    void PropositionQcmTeam(Guid partyId, Guid teamId, string response);
    void SetCurrentQcm(Guid partyId, QcmQuestionDto question);
}
```

**Responsabilité** : Gestion des états de jeu (buzz, réponses, scores, QCM)

### 3. **IPartyEventPublisher** - Publication d'événements
```csharp
public interface IPartyEventPublisher
{
    event EventHandler<PartyDto>? PartyChanged;
    void OnPartyChanged(PartyDto party);
}
```

**Responsabilité** : Publication et gestion des événements

### 4. **IThemeManager** - Gestion des thèmes
```csharp
public interface IThemeManager
{
    void LoadThemes(Guid partyId, IEnumerable<ThemeDto> themes);
    void SelectTheme(Guid partyId, Guid? teamId, ThemeDto theme);
    void ChoiceTheme(Guid partyId, Guid? teamId, ThemeDto theme);
    void ResetThemesChoices(Guid partyId);
}
```

**Responsabilité** : Chargement et gestion des thèmes de blind test

---

## 🏗️ Architecture résultante

### Avant
```
GameService
    ↓
PartyContext (god object - 257 lignes)
    ├─ Stockage
    ├─ État du jeu
    ├─ Événements
    └─ Thèmes
```

### Après
```
GameService
    ├─→ IPartyRepository      → PartyRepository      (stockage)
    ├─→ IPartyStateManager    → PartyStateManager    (état)
    ├─→ IPartyEventPublisher  → PartyEventPublisher  (événements)
    └─→ IThemeManager         → ThemeManager         (thèmes)
```

---

## 📂 Nouveaux fichiers créés

### Interfaces (`SpeedGameApp.Business/Services/Interfaces/`)
- ✅ `IPartyRepository.cs` - 66 lignes
- ✅ `IPartyStateManager.cs` - 64 lignes
- ✅ `IPartyEventPublisher.cs` - 18 lignes
- ✅ `IThemeManager.cs` - 36 lignes

### Implémentations (`SpeedGameApp.Business/Services/Implementations/`)
- ✅ `PartyRepository.cs` - 78 lignes
- ✅ `PartyStateManager.cs` - 104 lignes
- ✅ `PartyEventPublisher.cs` - 16 lignes
- ✅ `ThemeManager.cs` - 59 lignes

**Total : 8 nouveaux fichiers (441 lignes)**

---

## 🔧 Fichiers modifiés

### 1. `GameService.cs`
**Avant** - Dépendance directe sur PartyContext :
```csharp
public sealed class GameService(
    IServiceProvider serviceProvider,
    PartyAccessLayer partyAccessLayer,
    QuestionAccessLayer questionAccessLayer,
    ThemeAccessLayer themeAccessLayer,
    TimeProvider timeProvider)
{
    private readonly PartyContext context = serviceProvider.GetRequiredService<PartyContext>();

    public void BuzzTeam(Guid partyId, Guid teamId)
        => this.context.BuzzTeam(partyId, teamId);
}
```

**Après** - Injection des interfaces spécialisées :
```csharp
public sealed class GameService(
    IPartyRepository partyRepository,
    IPartyStateManager stateManager,
    IPartyEventPublisher eventPublisher,
    IThemeManager themeManager,
    PartyAccessLayer partyAccessLayer,
    QuestionAccessLayer questionAccessLayer,
    ThemeAccessLayer themeAccessLayer,
    TimeProvider timeProvider)
{
    public void BuzzTeam(Guid partyId, Guid teamId)
        => stateManager.BuzzTeam(partyId, teamId);
}
```

### 2. `BusinessExtensions.cs`
**Avant** :
```csharp
public static void AddBusinessServices(this IServiceCollection services)
{
    services.AddDalServices();
    services.TryAddTransient<GameService>();
    services.TryAddTransient<CsvService>();
    services.TryAddSingleton<PartyContext>();
    services.TryAddSingleton(TimeProvider.System);
}
```

**Après** :
```csharp
public static void AddBusinessServices(this IServiceCollection services)
{
    services.AddDalServices();

    // Core services
    services.TryAddTransient<GameService>();
    services.TryAddTransient<CsvService>();
    services.TryAddSingleton(TimeProvider.System);

    // Party management services (separated responsibilities)
    services.TryAddSingleton<IPartyEventPublisher, PartyEventPublisher>();
    services.TryAddSingleton<IPartyRepository, PartyRepository>();
    services.TryAddSingleton<IPartyStateManager, PartyStateManager>();
    services.TryAddSingleton<IThemeManager, ThemeManager>();

    // Legacy (kept for backwards compatibility, will be removed)
    services.TryAddSingleton<PartyContext>();
}
```

### 3. `PartyContext.cs`
Marqué comme **`[Obsolete]`** avec message explicatif :
```csharp
/// <remarks>
///     DEPRECATED: This class violates the Single Responsibility Principle and has been split into:
///     - <see cref="Services.Interfaces.IPartyRepository"/> for storage operations
///     - <see cref="Services.Interfaces.IPartyStateManager"/> for game state management
///     - <see cref="Services.Interfaces.IPartyEventPublisher"/> for event publishing
///     - <see cref="Services.Interfaces.IThemeManager"/> for theme management
///     This class is kept for backwards compatibility and will be removed in a future version.
/// </remarks>
[Obsolete("Use IPartyRepository, IPartyStateManager, IPartyEventPublisher, and IThemeManager instead.")]
internal sealed class PartyContext
```

---

## 🎯 Avantages du refactoring

### 1. **Single Responsibility Principle (SRP)**
Chaque service a une responsabilité claire et unique.

### 2. **Testabilité**
```csharp
// Avant - Difficile à mocker
[Test]
public void TestBuzzTeam()
{
    var context = new PartyContext(); // Tout le système en mémoire
    // ...
}

// Après - Facile à mocker
[Test]
public void TestBuzzTeam()
{
    var mockStateManager = new Mock<IPartyStateManager>();
    var mockRepository = new Mock<IPartyRepository>();
    // Tests unitaires isolés
}
```

### 3. **Dependency Inversion Principle (DIP)**
GameService dépend d'abstractions (interfaces) et non de classes concrètes.

### 4. **Séparation des concerns**
Modifications dans la gestion d'événements n'affectent pas le stockage.

### 5. **Code plus clair**
Chaque service est petit, focalisé, et facile à comprendre.

---

## 📊 Statistiques

| Métrique | Avant | Après | Amélioration |
|----------|-------|-------|--------------|
| Classe PartyContext | 257 lignes | 257 lignes (obsolète) | - |
| Nombre de classes | 1 | 9 (4 interfaces + 4 impls + 1 legacy) | +800% |
| Responsabilités par classe | 4 | 1 | -75% |
| Testabilité | Faible | Élevée | +++

 |
| Couplage GameService | Fort | Faible | +++ |
| Lignes de code total | 257 | 441 | +184 lignes |

**Note** : L'augmentation des lignes de code est normale et bénéfique. Le code est maintenant plus lisible, testable et maintenable.

---

## 🔄 Plan de migration complet

### Phase 1 : Création des interfaces ✅
- [x] Créer `IPartyRepository`
- [x] Créer `IPartyStateManager`
- [x] Créer `IPartyEventPublisher`
- [x] Créer `IThemeManager`

### Phase 2 : Implémentations ✅
- [x] Implémenter `PartyRepository`
- [x] Implémenter `PartyStateManager`
- [x] Implémenter `PartyEventPublisher`
- [x] Implémenter `ThemeManager`

### Phase 3 : Refactoring GameService ✅
- [x] Injecter les 4 nouvelles interfaces
- [x] Remplacer tous les appels à `PartyContext`
- [x] Supprimer la dépendance directe

### Phase 4 : Configuration DI ✅
- [x] Enregistrer les 4 nouveaux services
- [x] Marquer PartyContext comme obsolète
- [x] Ajouter commentaires de migration

### Phase 5 : Tests (À venir)
- [ ] Créer tests unitaires pour chaque service
- [ ] Tests d'intégration GameService
- [ ] Vérifier couverture de code

### Phase 6 : Suppression (Version future)
- [ ] Supprimer complètement `PartyContext.cs`
- [ ] Nettoyer les références obsolètes
- [ ] Mise à jour documentation

---

## 💡 Exemples d'utilisation

### Injection dans un service
```csharp
public class MyService(
    IPartyRepository repository,
    IPartyStateManager stateManager)
{
    public void DoSomething(Guid partyId)
    {
        // Récupérer une partie
        var party = repository.GetParty(partyId);

        // Modifier l'état
        stateManager.SetCurrentResponse(partyId, ResponseType.Buzzer);
    }
}
```

### Tests unitaires
```csharp
[Test]
public void AddPoints_ShouldUpdateTeamScore()
{
    // Arrange
    var mockRepo = new Mock<IPartyRepository>();
    var mockEventPublisher = new Mock<IPartyEventPublisher>();
    var stateManager = new PartyStateManager(mockRepo.Object);

    var team = new TeamDto(Guid.NewGuid(), "Team A");

    // Act
    stateManager.AddPoints(team, 10);

    // Assert
    Assert.AreEqual(10, team.Score);
}
```

---

## 🔍 Points d'attention

### PartyRepository a une dépendance sur IPartyEventPublisher
```csharp
public PartyRepository(IPartyEventPublisher eventPublisher)
```

C'est voulu : le repository publie des événements quand des parties sont ajoutées. C'est une responsabilité du repository de notifier les changements de stockage.

### PartyStateManager dépend de IPartyRepository
```csharp
public sealed class PartyStateManager(IPartyRepository repository)
```

C'est normal : le state manager a besoin d'accéder aux parties pour modifier leur état.

---

## 🎓 Principes SOLID appliqués

### ✅ **S** - Single Responsibility Principle
Chaque service a une seule raison de changer.

### ✅ **O** - Open/Closed Principle
Extensible via nouvelles implémentations d'interfaces sans modifier le code existant.

### ✅ **L** - Liskov Substitution Principle
Toutes les implémentations sont substituables via leurs interfaces.

### ✅ **I** - Interface Segregation Principle
Interfaces petites et focalisées, pas de méthodes inutiles.

### ✅ **D** - Dependency Inversion Principle
GameService dépend d'abstractions (interfaces), pas de classes concrètes.

---

## 📚 Ressources

- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [Single Responsibility Principle](https://en.wikipedia.org/wiki/Single-responsibility_principle)
- [Dependency Injection in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [Repository Pattern](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)

---

**Refactoring réalisé par Claude Code - 1er janvier 2026**

**Impact** : Amélioration majeure de la maintenabilité, testabilité et respect des principes SOLID.
