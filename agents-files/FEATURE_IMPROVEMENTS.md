# 🚀 SpeedGameApp - Propositions d'améliorations fonctionnelles

> Document créé le 2026-01-02
> État actuel : .NET 10, Blazor Server, Architecture SRP avec services spécialisés

---

## 📋 Table des matières

- [Vue d'ensemble](#vue-densemble)
- [Haute priorité](#-haute-priorité-impact-fort-effort-faible)
- [Moyenne priorité](#-moyenne-priorité-impact-moyen-effort-modéré)
- [Basse priorité](#-basse-priorité-nice-to-have)
- [Index par catégorie](#index-par-catégorie)

---

## Vue d'ensemble

### Contexte actuel

**SpeedGameApp** est un jeu de quiz multijoueur temps réel avec :
- ✅ 4 modes de jeu (Buzzer, Proposition, QCM, None)
- ✅ Gestion multi-équipes avec scoring
- ✅ Système de thèmes randomisés
- ✅ Synchronisation temps réel
- ✅ Interface admin et interface équipe séparées
- ✅ Pattern Result<T> pour gestion d'erreurs
- ✅ FluentValidation pour validation des entrées

### Objectif du document

Ce document liste **15 propositions d'améliorations** classées par priorité (ROI = Return On Investment). Chaque proposition contient :
- 🎯 **Problème identifié**
- ✅ **Solution proposée**
- 💻 **Implémentation technique**
- ⏱️ **Effort estimé**
- 💎 **Valeur apportée**

---

## 🔥 HAUTE PRIORITÉ (Impact fort, effort faible)

### 1️⃣ Affichage des erreurs utilisateur

**ID** : `FEAT-001`
**Priorité** : ⭐⭐⭐⭐⭐ (Critique)
**Effort** : 1-2h
**Valeur** : UX critique

#### 🎯 Problème

Actuellement, les erreurs de validation (nom vide, doublons, etc.) sont invisibles pour l'utilisateur. Le pattern `Result<T>` est en place mais les messages d'erreur ne sont jamais affichés.

**Fichiers concernés** :
- `Index.razor.cs` : ligne 35 `// TODO: Handle error case`
- `PartyTeamCreation.razor.cs` : ligne 18 `// TODO: Handle error case`

#### ✅ Solution

Afficher les messages d'erreur dans l'UI en utilisant les classes Bootstrap `alert alert-danger`.

#### 💻 Implémentation

**Étape 1 : Modifier Index.razor**
```razor
@page "/"
@using SpeedGameApp.Business.Services.Interfaces

<div class="container mt-5">
    <h1>Créer une nouvelle partie</h1>

    @if (!string.IsNullOrEmpty(errorMessage))
    {
        <div class="alert alert-danger alert-dismissible fade show" role="alert">
            <strong>Erreur :</strong> @errorMessage
            <button type="button" class="btn-close" @onclick="ClearError"></button>
        </div>
    }

    <div class="mb-3">
        <label for="partyName" class="form-label">Nom de la partie</label>
        <input type="text" class="form-control" id="partyName" @bind="partyName" />
    </div>

    <button class="btn btn-primary" @onclick="CreatePartyAsync">Créer</button>
</div>
```

**Étape 2 : Modifier Index.razor.cs**
```csharp
public partial class Index
{
    private string partyName = string.Empty;
    private string errorMessage = string.Empty;

    [Inject]
    public IPartyManagementService PartyManagementService { get; set; } = default!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    private async Task CreatePartyAsync(MouseEventArgs obj)
    {
        if (string.IsNullOrWhiteSpace(this.partyName))
        {
            this.errorMessage = "Le nom de la partie ne peut pas être vide.";
            return;
        }

        var cancellationTokenSource = new CancellationTokenSource();
        var result = await this.PartyManagementService.CreatePartyAsync(this.partyName, cancellationTokenSource.Token);

        if (result.IsSuccess)
        {
            this.NavigationManager.NavigateTo($"/party/{result.Value}/admin");
        }
        else
        {
            this.errorMessage = result.Error;
            await this.InvokeAsync(this.StateHasChanged);
        }
    }

    private void ClearError()
    {
        this.errorMessage = string.Empty;
    }
}
```

**Étape 3 : Répéter pour PartyTeamCreation.razor**

Même logique pour l'affichage d'erreur lors de la création d'équipe.

#### 📁 Fichiers à modifier

1. `SpeedGameApp/Pages/Index.razor` - Ajouter zone d'affichage erreur
2. `SpeedGameApp/Pages/Index.razor.cs` - Ajouter propriété errorMessage et logique
3. `SpeedGameApp/Pages/Game/PartyTeamCreation.razor` - Ajouter zone d'affichage erreur
4. `SpeedGameApp/Pages/Game/PartyTeamCreation.razor.cs` - Ajouter propriété errorMessage et logique

#### ✅ Critères d'acceptation

- [ ] Les erreurs de validation s'affichent en rouge avec icône
- [ ] L'utilisateur peut fermer le message d'erreur
- [ ] Le message disparaît après correction et nouvelle tentative
- [ ] Fonctionne sur Index et PartyTeamCreation

---

### 2️⃣ Confirmations pour actions destructives

**ID** : `FEAT-002`
**Priorité** : ⭐⭐⭐⭐⭐ (Critique)
**Effort** : 2-3h
**Valeur** : Sécurité, évite erreurs catastrophiques

#### 🎯 Problème

Les actions destructives (supprimer équipe, supprimer partie, tout supprimer) s'exécutent immédiatement sans confirmation. Un clic accidentel = données perdues.

**Fichiers concernés** :
- `Admin.razor.cs` : `DeleteParty()`, `DeleteAllParties()`, `DeleteDbPartyAsync()`
- `PartyAdmin.razor.cs` : `DeleteTeamAsync()`

#### ✅ Solution

Modal de confirmation Bootstrap avant toute action destructive.

#### 💻 Implémentation

**Étape 1 : Créer composant ConfirmDialog.razor**
```razor
@if (IsVisible)
{
    <div class="modal fade show d-block" tabindex="-1" style="background-color: rgba(0,0,0,0.5);">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">@Title</h5>
                    <button type="button" class="btn-close" @onclick="Cancel"></button>
                </div>
                <div class="modal-body">
                    <p>@Message</p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" @onclick="Cancel">Annuler</button>
                    <button type="button" class="btn btn-danger" @onclick="Confirm">@ConfirmText</button>
                </div>
            </div>
        </div>
    </div>
}

@code {
    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public string Title { get; set; } = "Confirmation";
    [Parameter] public string Message { get; set; } = "Êtes-vous sûr ?";
    [Parameter] public string ConfirmText { get; set; } = "Confirmer";
    [Parameter] public EventCallback OnConfirm { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private async Task Confirm()
    {
        await OnConfirm.InvokeAsync();
        IsVisible = false;
    }

    private async Task Cancel()
    {
        await OnCancel.InvokeAsync();
        IsVisible = false;
    }
}
```

**Étape 2 : Utiliser dans Admin.razor**
```razor
<ConfirmDialog
    IsVisible="@showDeleteDialog"
    Title="Supprimer la partie"
    Message="@($"Voulez-vous vraiment supprimer la partie '{partyToDelete?.Name}' ? Cette action est irréversible.")"
    ConfirmText="Supprimer"
    OnConfirm="ConfirmDeleteParty"
    OnCancel="CancelDelete" />

<button class="btn btn-danger" @onclick="() => ShowDeleteDialog(party.Id)">Supprimer</button>
```

**Étape 3 : Modifier Admin.razor.cs**
```csharp
private bool showDeleteDialog = false;
private PartyDto? partyToDelete = null;

private void ShowDeleteDialog(Guid partyId)
{
    partyToDelete = this.PartyManagementService.Parties[partyId];
    showDeleteDialog = true;
}

private void ConfirmDeleteParty()
{
    if (partyToDelete != null)
    {
        this.PartyManagementService.DeleteParty(partyToDelete.Id);
        partyToDelete = null;
    }
    showDeleteDialog = false;
}

private void CancelDelete()
{
    partyToDelete = null;
    showDeleteDialog = false;
}
```

#### 📁 Fichiers à créer/modifier

1. **Créer** `SpeedGameApp/Shared/Components/ConfirmDialog.razor` - Composant réutilisable
2. **Modifier** `SpeedGameApp/Pages/Admin/Admin.razor` - Utiliser ConfirmDialog
3. **Modifier** `SpeedGameApp/Pages/Admin/Admin.razor.cs` - Logique confirmation
4. **Modifier** `SpeedGameApp/Pages/PartyAdmin.razor` - Utiliser ConfirmDialog
5. **Modifier** `SpeedGameApp/Pages/PartyAdmin.razor.cs` - Logique confirmation

#### ✅ Critères d'acceptation

- [ ] Modal s'affiche avant suppression équipe/partie
- [ ] Bouton "Annuler" ferme sans supprimer
- [ ] Bouton "Confirmer" supprime et ferme
- [ ] Message personnalisé avec nom de la partie/équipe
- [ ] Fonctionne pour DeleteParty, DeleteAllParties, DeleteTeam, DeleteDbParty

---

### 3️⃣ Auto-save des scores

**ID** : `FEAT-003`
**Priorité** : ⭐⭐⭐⭐ (Haute)
**Effort** : 1h (déjà implémenté !)
**Valeur** : Sécurité des données

#### 🎯 Problème

Il faut manuellement cliquer "Sauvegarder" pour persister les scores en DB. Risque de perte si crash ou fermeture accidentelle.

#### ✅ Solution

**BONNE NOUVELLE** : L'auto-save est **déjà implémenté** ! 🎉

Dans `GameplayService.AddPointsAsync()` :
```csharp
public async Task AddPointsAsync(TeamDto teamDto, int points, CancellationToken cancellationToken)
{
    stateManager.AddPoints(teamDto, points);
    await partyManagementService.SavePartyAsync(teamDto.PartyId, cancellationToken); // ← AUTO-SAVE ICI
}
```

Chaque modification de score appelle automatiquement `SavePartyAsync()`.

#### 💻 Implémentation

**Aucune action requise** - Déjà fonctionnel.

**Optionnel** : Ajouter indicateur visuel "Sauvegarde automatique activée"

```razor
<div class="alert alert-info">
    <i class="bi bi-cloud-check"></i> Sauvegarde automatique activée - Vos scores sont protégés
</div>
```

#### 📁 Fichiers concernés

- `SpeedGameApp.Business/Services/Implementations/GameplayService.cs` (déjà OK)

#### ✅ Critères d'acceptation

- [x] Les scores sont sauvegardés automatiquement après chaque modification ✅ (Déjà fait)
- [ ] (Optionnel) Indicateur visuel "Auto-save ON" dans l'UI

---

### 4️⃣ Indicateurs de chargement

**ID** : `FEAT-004`
**Priorité** : ⭐⭐⭐⭐ (Haute)
**Effort** : 2-3h
**Valeur** : UX professionnelle

#### 🎯 Problème

Les opérations asynchrones (création partie, import CSV, chargement questions) n'ont aucun feedback visuel. L'utilisateur ne sait pas si son action est en cours.

#### ✅ Solution

Spinners Bootstrap pendant les opérations async + désactivation des boutons.

#### 💻 Implémentation

**Étape 1 : Créer composant LoadingButton.razor**
```razor
<button class="btn @ButtonClass" disabled="@IsLoading" @onclick="HandleClick">
    @if (IsLoading)
    {
        <span class="spinner-border spinner-border-sm me-2" role="status"></span>
        <span>@LoadingText</span>
    }
    else
    {
        @ChildContent
    }
</button>

@code {
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string ButtonClass { get; set; } = "btn-primary";
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public string LoadingText { get; set; } = "Chargement...";
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    private async Task HandleClick(MouseEventArgs e)
    {
        if (!IsLoading)
            await OnClick.InvokeAsync(e);
    }
}
```

**Étape 2 : Utiliser dans Index.razor**
```razor
<LoadingButton
    IsLoading="@isCreatingParty"
    LoadingText="Création en cours..."
    OnClick="CreatePartyAsync">
    Créer la partie
</LoadingButton>
```

**Étape 3 : Modifier Index.razor.cs**
```csharp
private bool isCreatingParty = false;

private async Task CreatePartyAsync(MouseEventArgs obj)
{
    if (string.IsNullOrWhiteSpace(this.partyName))
    {
        this.errorMessage = "Le nom de la partie ne peut pas être vide.";
        return;
    }

    isCreatingParty = true;
    await InvokeAsync(StateHasChanged);

    try
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var result = await this.PartyManagementService.CreatePartyAsync(this.partyName, cancellationTokenSource.Token);

        if (result.IsSuccess)
            this.NavigationManager.NavigateTo($"/party/{result.Value}/admin");
        else
            this.errorMessage = result.Error;
    }
    finally
    {
        isCreatingParty = false;
        await InvokeAsync(StateHasChanged);
    }
}
```

#### 📁 Fichiers à créer/modifier

1. **Créer** `SpeedGameApp/Shared/Components/LoadingButton.razor`
2. **Modifier** `SpeedGameApp/Pages/Index.razor` - Utiliser LoadingButton
3. **Modifier** `SpeedGameApp/Pages/Index.razor.cs` - Ajouter isLoading
4. **Modifier** `SpeedGameApp/Pages/Game/PartyTeamCreation.razor` - Utiliser LoadingButton
5. **Modifier** `SpeedGameApp/Pages/Admin/AdminQuestions.razor` - Loading pour import CSV

#### ✅ Critères d'acceptation

- [ ] Spinner visible pendant création partie
- [ ] Bouton désactivé pendant opération
- [ ] Spinner visible pendant création équipe
- [ ] Spinner visible pendant import CSV
- [ ] Texte change pendant chargement ("Création en cours...")

---

### 5️⃣ Historique des réponses par round

**ID** : `FEAT-005`
**Priorité** : ⭐⭐⭐⭐ (Haute)
**Effort** : 4-6h
**Valeur** : Traçabilité, replay, débug

#### 🎯 Problème

Impossible de revoir les réponses précédentes. Une fois le round terminé, les réponses disparaissent. Pas d'historique pour analyser la partie.

#### ✅ Solution

Ajouter un système de rounds avec historique complet des questions et réponses.

#### 💻 Implémentation

**Étape 1 : Créer modèle Round**
```csharp
namespace SpeedGameApp.Business.Services.Models;

public sealed record RoundDto
{
    public int Number { get; init; }
    public string Question { get; init; } = string.Empty;
    public ResponseType ResponseType { get; init; }
    public Dictionary<Guid, RoundTeamResponse> TeamResponses { get; init; } = [];
    public DateTime StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public Guid? WinningTeamId { get; init; }
}

public sealed record RoundTeamResponse
{
    public Guid TeamId { get; init; }
    public string TeamName { get; init; } = string.Empty;
    public string Response { get; init; } = string.Empty;
    public bool IsCorrect { get; init; }
    public int PointsAwarded { get; init; }
    public DateTime ResponseTime { get; init; }
}
```

**Étape 2 : Ajouter à PartyDto**
```csharp
public sealed class PartyDto
{
    // ... propriétés existantes ...

    public List<RoundDto> Rounds { get; set; } = [];
    public int CurrentRoundNumber => Rounds.Count + 1;
}
```

**Étape 3 : Créer service IRoundHistoryService**
```csharp
namespace SpeedGameApp.Business.Services.Interfaces;

public interface IRoundHistoryService
{
    void StartRound(Guid partyId, string question, ResponseType responseType);
    void EndRound(Guid partyId, Guid? winningTeamId);
    void RecordResponse(Guid partyId, Guid teamId, string response, bool isCorrect, int pointsAwarded);
    IEnumerable<RoundDto> GetRoundHistory(Guid partyId);
    RoundDto? GetCurrentRound(Guid partyId);
}
```

**Étape 4 : Implémenter RoundHistoryService**
```csharp
namespace SpeedGameApp.Business.Services.Implementations;

public sealed class RoundHistoryService(
    IPartyRepository partyRepository,
    TimeProvider timeProvider) : IRoundHistoryService
{
    public void StartRound(Guid partyId, string question, ResponseType responseType)
    {
        if (!partyRepository.Parties.TryGetValue(partyId, out var party))
            return;

        var round = new RoundDto
        {
            Number = party.CurrentRoundNumber,
            Question = question,
            ResponseType = responseType,
            StartTime = timeProvider.GetUtcNow().UtcDateTime,
            TeamResponses = new Dictionary<Guid, RoundTeamResponse>()
        };

        party.Rounds.Add(round);
    }

    public void EndRound(Guid partyId, Guid? winningTeamId)
    {
        var currentRound = GetCurrentRound(partyId);
        if (currentRound == null)
            return;

        var updatedRound = currentRound with
        {
            EndTime = timeProvider.GetUtcNow().UtcDateTime,
            WinningTeamId = winningTeamId
        };

        // Remplacer le round
        if (partyRepository.Parties.TryGetValue(partyId, out var party))
        {
            party.Rounds[party.Rounds.Count - 1] = updatedRound;
        }
    }

    public void RecordResponse(Guid partyId, Guid teamId, string response, bool isCorrect, int pointsAwarded)
    {
        var currentRound = GetCurrentRound(partyId);
        if (currentRound == null)
            return;

        if (!partyRepository.Parties.TryGetValue(partyId, out var party))
            return;

        if (!party.Teams.TryGetValue(teamId, out var team))
            return;

        var teamResponse = new RoundTeamResponse
        {
            TeamId = teamId,
            TeamName = team.Name,
            Response = response,
            IsCorrect = isCorrect,
            PointsAwarded = pointsAwarded,
            ResponseTime = timeProvider.GetUtcNow().UtcDateTime
        };

        currentRound.TeamResponses[teamId] = teamResponse;
    }

    public IEnumerable<RoundDto> GetRoundHistory(Guid partyId)
    {
        if (!partyRepository.Parties.TryGetValue(partyId, out var party))
            return [];

        return party.Rounds;
    }

    public RoundDto? GetCurrentRound(Guid partyId)
    {
        if (!partyRepository.Parties.TryGetValue(partyId, out var party))
            return null;

        return party.Rounds.LastOrDefault(r => r.EndTime == null);
    }
}
```

**Étape 5 : Créer page RoundHistory.razor**
```razor
@page "/party/{PartyId:guid}/history"
@inherits PartyPageBase

<h3>Historique des rounds - @CurrentParty.Name</h3>

@if (!rounds.Any())
{
    <div class="alert alert-info">Aucun round joué pour le moment.</div>
}
else
{
    <div class="accordion" id="roundAccordion">
        @foreach (var round in rounds.OrderByDescending(r => r.Number))
        {
            <div class="accordion-item">
                <h2 class="accordion-header">
                    <button class="accordion-button @(round.Number == 1 ? "" : "collapsed")"
                            type="button"
                            data-bs-toggle="collapse"
                            data-bs-target="#round-@round.Number">
                        Round @round.Number - @round.Question
                        @if (round.WinningTeamId.HasValue)
                        {
                            <span class="badge bg-success ms-2">
                                Gagné par @CurrentParty.Teams[round.WinningTeamId.Value].Name
                            </span>
                        }
                    </button>
                </h2>
                <div id="round-@round.Number" class="accordion-collapse collapse @(round.Number == 1 ? "show" : "")" data-bs-parent="#roundAccordion">
                    <div class="accordion-body">
                        <p><strong>Type:</strong> @round.ResponseType</p>
                        <p><strong>Début:</strong> @round.StartTime.ToString("HH:mm:ss")</p>
                        @if (round.EndTime.HasValue)
                        {
                            <p><strong>Fin:</strong> @round.EndTime.Value.ToString("HH:mm:ss")</p>
                            <p><strong>Durée:</strong> @((round.EndTime.Value - round.StartTime).TotalSeconds.ToString("F1"))s</p>
                        }

                        <h5>Réponses des équipes:</h5>
                        <table class="table table-sm">
                            <thead>
                                <tr>
                                    <th>Équipe</th>
                                    <th>Réponse</th>
                                    <th>Résultat</th>
                                    <th>Points</th>
                                    <th>Temps</th>
                                </tr>
                            </thead>
                            <tbody>
                                @foreach (var response in round.TeamResponses.Values.OrderBy(r => r.ResponseTime))
                                {
                                    <tr class="@(response.IsCorrect ? "table-success" : "table-danger")">
                                        <td>@response.TeamName</td>
                                        <td>@response.Response</td>
                                        <td>
                                            @if (response.IsCorrect)
                                            {
                                                <span class="badge bg-success">✓ Correct</span>
                                            }
                                            else
                                            {
                                                <span class="badge bg-danger">✗ Incorrect</span>
                                            }
                                        </td>
                                        <td>@(response.PointsAwarded > 0 ? "+" : "")@response.PointsAwarded</td>
                                        <td>@((response.ResponseTime - round.StartTime).TotalSeconds.ToString("F1"))s</td>
                                    </tr>
                                }
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        }
    </div>
}

<a href="/party/@PartyId/admin" class="btn btn-secondary mt-3">Retour à l'admin</a>

@code {
    private List<RoundDto> rounds = [];

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        rounds = RoundHistoryService.GetRoundHistory(PartyId).ToList();
    }
}
```

**Étape 6 : Intégrer dans GameplayService**

Modifier `GameplayService.AddPointsAsync()` pour enregistrer dans l'historique :
```csharp
public async Task AddPointsAsync(TeamDto teamDto, int points, CancellationToken cancellationToken)
{
    stateManager.AddPoints(teamDto, points);

    // Enregistrer dans l'historique
    roundHistoryService.RecordResponse(
        teamDto.PartyId,
        teamDto.Id,
        teamDto.Response,
        isCorrect: points > 0,
        pointsAwarded: points
    );

    await partyManagementService.SavePartyAsync(teamDto.PartyId, cancellationToken);
}
```

#### 📁 Fichiers à créer/modifier

1. **Créer** `SpeedGameApp.Business/Services/Models/RoundDto.cs`
2. **Créer** `SpeedGameApp.Business/Services/Interfaces/IRoundHistoryService.cs`
3. **Créer** `SpeedGameApp.Business/Services/Implementations/RoundHistoryService.cs`
4. **Créer** `SpeedGameApp/Pages/RoundHistory.razor`
5. **Créer** `SpeedGameApp/Pages/RoundHistory.razor.cs`
6. **Modifier** `SpeedGameApp.Business/Data/PartyDto.cs` - Ajouter List<RoundDto> Rounds
7. **Modifier** `SpeedGameApp.Business/Services/Implementations/GameplayService.cs` - Injecter IRoundHistoryService
8. **Modifier** `SpeedGameApp.Business/Extensions/BusinessExtensions.cs` - Enregistrer IRoundHistoryService
9. **Modifier** `SpeedGameApp/Pages/PartyAdmin.razor` - Ajouter bouton "Historique"

#### ✅ Critères d'acceptation

- [ ] Chaque round est enregistré avec question, type, timestamps
- [ ] Les réponses de toutes les équipes sont enregistrées
- [ ] Page historique affiche tous les rounds en accordéon
- [ ] Indication visuelle correct/incorrect par réponse
- [ ] Points attribués visibles par réponse
- [ ] Durée de réponse calculée (temps écoulé depuis début round)
- [ ] Équipe gagnante marquée par round
- [ ] Bouton "Historique" dans PartyAdmin

---

## 💡 MOYENNE PRIORITÉ (Impact moyen, effort modéré)

### 6️⃣ Timer par question

**ID** : `FEAT-006`
**Priorité** : ⭐⭐⭐ (Moyenne)
**Effort** : 6-8h
**Valeur** : Dynamise le jeu, empêche la triche

#### 🎯 Problème

Pas de pression temporelle sur les modes Buzzer et Proposition. Les équipes peuvent prendre tout leur temps, ce qui peut ralentir le jeu.

#### ✅ Solution

Compteur dégressif configurable (par défaut 30s) avec auto-reset après timeout.

#### 💻 Implémentation

**Étape 1 : Ajouter propriétés Timer à PartyDto**
```csharp
public sealed class PartyDto
{
    // ... propriétés existantes ...

    public DateTime? QuestionStartTime { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
    public bool IsTimerActive { get; set; }
}
```

**Étape 2 : Créer service ITimerService**
```csharp
namespace SpeedGameApp.Business.Services.Interfaces;

public interface ITimerService
{
    void StartTimer(Guid partyId, int seconds);
    void StopTimer(Guid partyId);
    int GetRemainingSeconds(Guid partyId);
    bool IsExpired(Guid partyId);
    event EventHandler<Guid> TimerExpired;
}
```

**Étape 3 : Implémenter TimerService**
```csharp
namespace SpeedGameApp.Business.Services.Implementations;

public sealed class TimerService(
    IPartyRepository partyRepository,
    TimeProvider timeProvider) : ITimerService, IDisposable
{
    private readonly Dictionary<Guid, System.Threading.Timer> _timers = [];
    public event EventHandler<Guid>? TimerExpired;

    public void StartTimer(Guid partyId, int seconds)
    {
        if (!partyRepository.Parties.TryGetValue(partyId, out var party))
            return;

        StopTimer(partyId); // Arrêter timer existant

        party.QuestionStartTime = timeProvider.GetUtcNow().UtcDateTime;
        party.TimeoutSeconds = seconds;
        party.IsTimerActive = true;

        var timer = new System.Threading.Timer(_ => OnTimerExpired(partyId), null, seconds * 1000, Timeout.Infinite);
        _timers[partyId] = timer;
    }

    public void StopTimer(Guid partyId)
    {
        if (_timers.TryGetValue(partyId, out var timer))
        {
            timer.Dispose();
            _timers.Remove(partyId);
        }

        if (partyRepository.Parties.TryGetValue(partyId, out var party))
        {
            party.IsTimerActive = false;
        }
    }

    public int GetRemainingSeconds(Guid partyId)
    {
        if (!partyRepository.Parties.TryGetValue(partyId, out var party))
            return 0;

        if (!party.IsTimerActive || !party.QuestionStartTime.HasValue)
            return 0;

        var elapsed = timeProvider.GetUtcNow().UtcDateTime - party.QuestionStartTime.Value;
        var remaining = party.TimeoutSeconds - (int)elapsed.TotalSeconds;
        return Math.Max(0, remaining);
    }

    public bool IsExpired(Guid partyId)
    {
        return GetRemainingSeconds(partyId) == 0;
    }

    private void OnTimerExpired(Guid partyId)
    {
        StopTimer(partyId);
        TimerExpired?.Invoke(this, partyId);
    }

    public void Dispose()
    {
        foreach (var timer in _timers.Values)
        {
            timer.Dispose();
        }
        _timers.Clear();
    }
}
```

**Étape 4 : Créer composant CountdownTimer.razor**
```razor
@implements IDisposable
@inject ITimerService TimerService

<div class="countdown-timer @(GetTimerClass())">
    <div class="timer-display">
        <span class="timer-value">@remainingSeconds</span>
        <span class="timer-label">sec</span>
    </div>
    <div class="progress" style="height: 5px;">
        <div class="progress-bar @GetProgressBarClass()"
             role="progressbar"
             style="width: @GetProgressPercentage()%">
        </div>
    </div>
</div>

@code {
    [Parameter] public Guid PartyId { get; set; }
    [Parameter] public int TotalSeconds { get; set; } = 30;

    private int remainingSeconds;
    private System.Threading.Timer? updateTimer;

    protected override void OnInitialized()
    {
        remainingSeconds = TimerService.GetRemainingSeconds(PartyId);
        updateTimer = new System.Threading.Timer(_ => UpdateDisplay(), null, 0, 100);
        TimerService.TimerExpired += OnTimerExpired;
    }

    private void UpdateDisplay()
    {
        var newRemaining = TimerService.GetRemainingSeconds(PartyId);
        if (newRemaining != remainingSeconds)
        {
            remainingSeconds = newRemaining;
            InvokeAsync(StateHasChanged);
        }
    }

    private void OnTimerExpired(object? sender, Guid partyId)
    {
        if (partyId == PartyId)
        {
            InvokeAsync(StateHasChanged);
        }
    }

    private string GetTimerClass()
    {
        return remainingSeconds switch
        {
            <= 5 => "timer-critical",
            <= 10 => "timer-warning",
            _ => "timer-normal"
        };
    }

    private string GetProgressBarClass()
    {
        return remainingSeconds switch
        {
            <= 5 => "bg-danger",
            <= 10 => "bg-warning",
            _ => "bg-success"
        };
    }

    private double GetProgressPercentage()
    {
        if (TotalSeconds == 0) return 0;
        return (double)remainingSeconds / TotalSeconds * 100;
    }

    public void Dispose()
    {
        updateTimer?.Dispose();
        TimerService.TimerExpired -= OnTimerExpired;
    }
}

<style>
    .countdown-timer {
        text-align: center;
        padding: 1rem;
        border-radius: 8px;
        margin-bottom: 1rem;
    }

    .timer-normal {
        background-color: #e8f5e9;
    }

    .timer-warning {
        background-color: #fff3e0;
        animation: pulse-warning 1s infinite;
    }

    .timer-critical {
        background-color: #ffebee;
        animation: pulse-critical 0.5s infinite;
    }

    .timer-display {
        font-size: 3rem;
        font-weight: bold;
        font-family: 'Courier New', monospace;
    }

    .timer-label {
        font-size: 1rem;
        margin-left: 0.5rem;
    }

    @@keyframes pulse-warning {
        0%, 100% { transform: scale(1); }
        50% { transform: scale(1.05); }
    }

    @@keyframes pulse-critical {
        0%, 100% { transform: scale(1); }
        50% { transform: scale(1.1); }
    }
</style>
```

**Étape 5 : Intégrer dans PartyAdmin.razor**
```razor
@if (CurrentParty.IsTimerActive)
{
    <CountdownTimer PartyId="@PartyId" TotalSeconds="@CurrentParty.TimeoutSeconds" />
}

<div class="mb-3">
    <label class="form-label">Durée du timer (secondes)</label>
    <input type="number" class="form-control" @bind="timerSeconds" min="5" max="120" />
</div>

<button class="btn btn-primary" @onclick="StartQuestionTimer">
    Démarrer timer (@timerSeconds sec)
</button>
<button class="btn btn-secondary" @onclick="StopQuestionTimer">
    Arrêter timer
</button>
```

**Étape 6 : Modifier PartyAdmin.razor.cs**
```csharp
[Inject]
public ITimerService TimerService { get; set; } = default!;

private int timerSeconds = 30;

protected override void OnInitialized()
{
    base.OnInitialized();
    TimerService.TimerExpired += OnTimerExpired;
}

private void StartQuestionTimer()
{
    TimerService.StartTimer(PartyId, timerSeconds);
}

private void StopQuestionTimer()
{
    TimerService.StopTimer(PartyId);
}

private async void OnTimerExpired(object? sender, Guid partyId)
{
    if (partyId == PartyId)
    {
        // Auto-reset des réponses quand le timer expire
        GameplayService.ResetTeam(PartyId);
        await InvokeAsync(StateHasChanged);
    }
}

public override void Dispose()
{
    TimerService.TimerExpired -= OnTimerExpired;
    base.Dispose();
}
```

#### 📁 Fichiers à créer/modifier

1. **Créer** `SpeedGameApp.Business/Services/Interfaces/ITimerService.cs`
2. **Créer** `SpeedGameApp.Business/Services/Implementations/TimerService.cs`
3. **Créer** `SpeedGameApp/Shared/Components/CountdownTimer.razor`
4. **Modifier** `SpeedGameApp.Business/Data/PartyDto.cs` - Ajouter propriétés timer
5. **Modifier** `SpeedGameApp/Pages/PartyAdmin.razor` - Intégrer timer
6. **Modifier** `SpeedGameApp/Pages/PartyAdmin.razor.cs` - Logique timer
7. **Modifier** `SpeedGameApp/Pages/Game/PartyTeamPlay.razor` - Afficher timer côté équipe
8. **Modifier** `SpeedGameApp.Business/Extensions/BusinessExtensions.cs` - Enregistrer ITimerService

#### ✅ Critères d'acceptation

- [ ] Admin peut démarrer un timer avec durée configurable (5-120 sec)
- [ ] Timer visible côté admin ET côté équipes
- [ ] Changement de couleur selon temps restant (vert > 10s, orange 5-10s, rouge < 5s)
- [ ] Animation pulse quand < 10s
- [ ] Auto-reset des réponses quand timer expire
- [ ] Admin peut arrêter le timer manuellement
- [ ] Barre de progression visuelle
- [ ] Timer persiste entre rafraîchissements de page

---

### 7️⃣ Filtrage questions QCM

**ID** : `FEAT-007`
**Priorité** : ⭐⭐⭐ (Moyenne)
**Effort** : 3-4h
**Valeur** : Contrôle précis du niveau de jeu

#### 🎯 Problème

`SetRandomQcm()` choisit n'importe quelle question de la base. Impossible de filtrer par thème ou difficulté pour adapter le niveau.

#### ✅ Solution

Ajouter paramètres optionnels `themeId` et `difficulty` à `SetRandomQcm()` avec UI de filtrage.

#### 💻 Implémentation

**Étape 1 : Modifier IQuestionAccessLayer**
```csharp
namespace SpeedGameApp.DataAccessLayer.Interfaces;

public interface IQuestionAccessLayer
{
    QcmQuestion GetRandom();
    QcmQuestion GetRandom(Guid? themeId, Difficulty? difficulty);
    List<Theme> GetAllThemes();
}
```

**Étape 2 : Implémenter filtrage dans QuestionAccessLayer**
```csharp
public QcmQuestion GetRandom(Guid? themeId, Difficulty? difficulty)
{
    IQueryable<QcmQuestion> query = context.Questions.Include(q => q.Theme);

    // Filtrer par thème si spécifié
    if (themeId.HasValue)
        query = query.Where(q => q.Theme.Id == themeId.Value);

    // Filtrer par difficulté si spécifié
    if (difficulty.HasValue)
        query = query.Where(q => q.Difficulty == difficulty.Value);

    var questions = query.ToList();

    if (questions.Count == 0)
        throw new InvalidOperationException("Aucune question ne correspond aux critères de filtrage.");

    var randomIndex = Random.Shared.Next(questions.Count);
    return questions[randomIndex];
}

public List<Theme> GetAllThemes()
{
    return context.Themes.ToList();
}
```

**Étape 3 : Modifier IQcmService**
```csharp
namespace SpeedGameApp.Business.Services.Interfaces;

public interface IQcmService
{
    void SetRandomQcm(Guid partyId);
    void SetRandomQcm(Guid partyId, Guid? themeId, Difficulty? difficulty);
    void PropositionQcmTeam(Guid partyId, Guid teamId, string response);
}
```

**Étape 4 : Modifier QcmService**
```csharp
public void SetRandomQcm(Guid partyId, Guid? themeId, Difficulty? difficulty)
{
    var question = questionAccessLayer.GetRandom(themeId, difficulty);
    stateManager.SetCurrentQcm(partyId, QcmQuestionDto.FromQcmQuestion(question));
}

public void SetRandomQcm(Guid partyId)
{
    SetRandomQcm(partyId, null, null);
}
```

**Étape 5 : Modifier PartyAdmin.razor**
```razor
<div class="card mb-3">
    <div class="card-header">
        <h5>Sélection QCM</h5>
    </div>
    <div class="card-body">
        <div class="row">
            <div class="col-md-6 mb-3">
                <label class="form-label">Thème</label>
                <select class="form-select" @bind="selectedThemeId">
                    <option value="">Tous les thèmes</option>
                    @foreach (var theme in availableThemes)
                    {
                        <option value="@theme.Id">@theme.Name</option>
                    }
                </select>
            </div>
            <div class="col-md-6 mb-3">
                <label class="form-label">Difficulté</label>
                <select class="form-select" @bind="selectedDifficulty">
                    <option value="">Toutes difficultés</option>
                    <option value="@Difficulty.Facile">Facile</option>
                    <option value="@Difficulty.Moyenne">Moyenne</option>
                    <option value="@Difficulty.Difficile">Difficile</option>
                </select>
            </div>
        </div>
        <button class="btn btn-primary" @onclick="RandomizeQcmWithFilters">
            Question aléatoire
        </button>
        @if (!string.IsNullOrEmpty(qcmError))
        {
            <div class="alert alert-warning mt-2">@qcmError</div>
        }
    </div>
</div>
```

**Étape 6 : Modifier PartyAdmin.razor.cs**
```csharp
private List<Theme> availableThemes = [];
private string selectedThemeId = string.Empty;
private string selectedDifficulty = string.Empty;
private string qcmError = string.Empty;

protected override async Task OnInitializedAsync()
{
    await base.OnInitializedAsync();
    availableThemes = await QuestionAccessLayer.GetAllThemesAsync(); // Nouvelle méthode
}

private void RandomizeQcmWithFilters()
{
    try
    {
        qcmError = string.Empty;

        Guid? themeId = string.IsNullOrEmpty(selectedThemeId) ? null : Guid.Parse(selectedThemeId);
        Difficulty? difficulty = string.IsNullOrEmpty(selectedDifficulty) ? null : Enum.Parse<Difficulty>(selectedDifficulty);

        QcmService.SetRandomQcm(PartyId, themeId, difficulty);
    }
    catch (InvalidOperationException ex)
    {
        qcmError = ex.Message;
    }
}
```

#### 📁 Fichiers à modifier

1. **Modifier** `SpeedGameApp.DataAccessLayer/Interfaces/IQuestionAccessLayer.cs` - Ajouter surcharge avec filtres
2. **Modifier** `SpeedGameApp.DataAccessLayer/AccessLayers/QuestionAccessLayer.cs` - Implémenter filtrage
3. **Modifier** `SpeedGameApp.Business/Services/Interfaces/IQcmService.cs` - Ajouter surcharge
4. **Modifier** `SpeedGameApp.Business/Services/Implementations/QcmService.cs` - Implémenter filtrage
5. **Modifier** `SpeedGameApp/Pages/PartyAdmin.razor` - Ajouter UI filtres
6. **Modifier** `SpeedGameApp/Pages/PartyAdmin.razor.cs` - Logique filtres

#### ✅ Critères d'acceptation

- [ ] Dropdown "Thème" affiche tous les thèmes disponibles
- [ ] Dropdown "Difficulté" affiche Facile/Moyenne/Difficile
- [ ] Option "Tous" pour thème et difficulté
- [ ] Question aléatoire respecte les filtres sélectionnés
- [ ] Message d'erreur si aucune question ne correspond aux critères
- [ ] Filtres peuvent se combiner (thème ET difficulté)

---

### 8️⃣ Création de questions via UI

**ID** : `FEAT-008`
**Priorité** : ⭐⭐⭐ (Moyenne)
**Effort** : 4-5h
**Valeur** : Flexibilité pour créer questions à la volée

#### 🎯 Problème

CSV import only = lourd pour ajouter 1-2 questions. Pas pratique pour ajustements rapides.

#### ✅ Solution

Formulaire de création de question dans `/admin/questions` avec validation.

#### 💻 Implémentation

**Étape 1 : Créer IQuestionManagementService**
```csharp
namespace SpeedGameApp.Business.Services.Interfaces;

public interface IQuestionManagementService
{
    Task<Result> CreateQuestionAsync(string question, string option1, string option2, string option3, string option4, int correctOption, Guid themeId, Difficulty difficulty);
    Task<Result> UpdateQuestionAsync(Guid questionId, string question, string option1, string option2, string option3, string option4, int correctOption, Difficulty difficulty);
    Task<Result> DeleteQuestionAsync(Guid questionId);
}
```

**Étape 2 : Créer validateur QuestionValidator**
```csharp
namespace SpeedGameApp.Business.Validators;

public sealed class QuestionValidator : AbstractValidator<CreateQuestionRequest>
{
    public QuestionValidator()
    {
        RuleFor(x => x.Question)
            .NotEmpty().WithMessage("La question est requise.")
            .MinimumLength(10).WithMessage("La question doit contenir au moins 10 caractères.")
            .MaximumLength(500).WithMessage("La question ne peut pas dépasser 500 caractères.");

        RuleFor(x => x.Option1)
            .NotEmpty().WithMessage("L'option 1 est requise.");

        RuleFor(x => x.Option2)
            .NotEmpty().WithMessage("L'option 2 est requise.");

        RuleFor(x => x.Option3)
            .NotEmpty().WithMessage("L'option 3 est requise.");

        RuleFor(x => x.Option4)
            .NotEmpty().WithMessage("L'option 4 est requise.");

        RuleFor(x => x.CorrectOption)
            .InclusiveBetween(1, 4).WithMessage("La bonne réponse doit être entre 1 et 4.");

        RuleFor(x => x.ThemeId)
            .NotEmpty().WithMessage("Le thème est requis.");
    }
}

public record CreateQuestionRequest(
    string Question,
    string Option1,
    string Option2,
    string Option3,
    string Option4,
    int CorrectOption,
    Guid ThemeId,
    Difficulty Difficulty
);
```

**Étape 3 : Implémenter QuestionManagementService**
```csharp
namespace SpeedGameApp.Business.Services.Implementations;

public sealed class QuestionManagementService(
    SpeedGameDbContext context,
    IValidator<CreateQuestionRequest> validator) : IQuestionManagementService
{
    public async Task<Result> CreateQuestionAsync(
        string question,
        string option1,
        string option2,
        string option3,
        string option4,
        int correctOption,
        Guid themeId,
        Difficulty difficulty)
    {
        var request = new CreateQuestionRequest(question, option1, option2, option3, option4, correctOption, themeId, difficulty);

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors[0].ErrorMessage);

        var theme = await context.Themes.FindAsync(themeId);
        if (theme == null)
            return Result.Failure("Le thème spécifié n'existe pas.");

        var qcmQuestion = new QcmQuestion
        {
            Question = question,
            Option1 = option1,
            Option2 = option2,
            Option3 = option3,
            Option4 = option4,
            Response = correctOption switch
            {
                1 => option1,
                2 => option2,
                3 => option3,
                4 => option4,
                _ => throw new ArgumentException("Numéro d'option invalide")
            },
            Difficulty = difficulty,
            Theme = new QcmTheme { Id = theme.Id, Name = theme.Name }
        };

        context.Questions.Add(qcmQuestion);
        await context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> UpdateQuestionAsync(Guid questionId, string question, string option1, string option2, string option3, string option4, int correctOption, Difficulty difficulty)
    {
        var existingQuestion = await context.Questions.FindAsync(questionId);
        if (existingQuestion == null)
            return Result.Failure("Question introuvable.");

        // Validation similaire...

        existingQuestion.Question = question;
        existingQuestion.Option1 = option1;
        existingQuestion.Option2 = option2;
        existingQuestion.Option3 = option3;
        existingQuestion.Option4 = option4;
        existingQuestion.Response = correctOption switch
        {
            1 => option1,
            2 => option2,
            3 => option3,
            4 => option4,
            _ => throw new ArgumentException("Numéro d'option invalide")
        };
        existingQuestion.Difficulty = difficulty;

        await context.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> DeleteQuestionAsync(Guid questionId)
    {
        var question = await context.Questions.FindAsync(questionId);
        if (question == null)
            return Result.Failure("Question introuvable.");

        context.Questions.Remove(question);
        await context.SaveChangesAsync();
        return Result.Success();
    }
}
```

**Étape 4 : Modifier AdminQuestions.razor**
```razor
@page "/admin/questions"
@inject IQuestionManagementService QuestionService
@inject IQuestionAccessLayer QuestionAccessLayer

<h3>Gestion des questions</h3>

<!-- Formulaire de création -->
<div class="card mb-4">
    <div class="card-header">
        <h5>Créer une nouvelle question</h5>
    </div>
    <div class="card-body">
        @if (!string.IsNullOrEmpty(createError))
        {
            <div class="alert alert-danger">@createError</div>
        }
        @if (createSuccess)
        {
            <div class="alert alert-success">Question créée avec succès !</div>
        }

        <div class="mb-3">
            <label class="form-label">Question</label>
            <textarea class="form-control" rows="3" @bind="newQuestion"></textarea>
        </div>

        <div class="row">
            <div class="col-md-6 mb-3">
                <label class="form-label">Option 1</label>
                <input type="text" class="form-control" @bind="newOption1" />
            </div>
            <div class="col-md-6 mb-3">
                <label class="form-label">Option 2</label>
                <input type="text" class="form-control" @bind="newOption2" />
            </div>
        </div>

        <div class="row">
            <div class="col-md-6 mb-3">
                <label class="form-label">Option 3</label>
                <input type="text" class="form-control" @bind="newOption3" />
            </div>
            <div class="col-md-6 mb-3">
                <label class="form-label">Option 4</label>
                <input type="text" class="form-control" @bind="newOption4" />
            </div>
        </div>

        <div class="row">
            <div class="col-md-4 mb-3">
                <label class="form-label">Bonne réponse</label>
                <select class="form-select" @bind="correctOption">
                    <option value="1">Option 1</option>
                    <option value="2">Option 2</option>
                    <option value="3">Option 3</option>
                    <option value="4">Option 4</option>
                </select>
            </div>
            <div class="col-md-4 mb-3">
                <label class="form-label">Thème</label>
                <select class="form-select" @bind="selectedThemeId">
                    <option value="">-- Sélectionner --</option>
                    @foreach (var theme in themes)
                    {
                        <option value="@theme.Id">@theme.Name</option>
                    }
                </select>
            </div>
            <div class="col-md-4 mb-3">
                <label class="form-label">Difficulté</label>
                <select class="form-select" @bind="selectedDifficulty">
                    <option value="@Difficulty.Facile">Facile</option>
                    <option value="@Difficulty.Moyenne">Moyenne</option>
                    <option value="@Difficulty.Difficile">Difficile</option>
                </select>
            </div>
        </div>

        <button class="btn btn-primary" @onclick="CreateQuestion">
            <i class="bi bi-plus-circle"></i> Créer la question
        </button>
    </div>
</div>

<!-- Import CSV existant -->
<div class="card">
    <div class="card-header">
        <h5>Importer depuis CSV</h5>
    </div>
    <div class="card-body">
        <!-- Code d'import CSV existant -->
    </div>
</div>

@code {
    private string newQuestion = string.Empty;
    private string newOption1 = string.Empty;
    private string newOption2 = string.Empty;
    private string newOption3 = string.Empty;
    private string newOption4 = string.Empty;
    private int correctOption = 1;
    private string selectedThemeId = string.Empty;
    private Difficulty selectedDifficulty = Difficulty.Facile;

    private List<Theme> themes = [];
    private string createError = string.Empty;
    private bool createSuccess = false;

    protected override async Task OnInitializedAsync()
    {
        themes = await QuestionAccessLayer.GetAllThemesAsync();
    }

    private async Task CreateQuestion()
    {
        createError = string.Empty;
        createSuccess = false;

        if (string.IsNullOrEmpty(selectedThemeId))
        {
            createError = "Veuillez sélectionner un thème.";
            return;
        }

        var result = await QuestionService.CreateQuestionAsync(
            newQuestion,
            newOption1,
            newOption2,
            newOption3,
            newOption4,
            correctOption,
            Guid.Parse(selectedThemeId),
            selectedDifficulty
        );

        if (result.IsSuccess)
        {
            createSuccess = true;
            // Réinitialiser le formulaire
            newQuestion = string.Empty;
            newOption1 = string.Empty;
            newOption2 = string.Empty;
            newOption3 = string.Empty;
            newOption4 = string.Empty;
            correctOption = 1;
        }
        else
        {
            createError = result.Error;
        }
    }
}
```

#### 📁 Fichiers à créer/modifier

1. **Créer** `SpeedGameApp.Business/Services/Interfaces/IQuestionManagementService.cs`
2. **Créer** `SpeedGameApp.Business/Services/Implementations/QuestionManagementService.cs`
3. **Créer** `SpeedGameApp.Business/Validators/QuestionValidator.cs`
4. **Modifier** `SpeedGameApp/Pages/Admin/AdminQuestions.razor` - Ajouter formulaire création
5. **Modifier** `SpeedGameApp/Pages/Admin/AdminQuestions.razor.cs` - Logique création
6. **Modifier** `SpeedGameApp.Business/Extensions/BusinessExtensions.cs` - Enregistrer IQuestionManagementService
7. **Modifier** `SpeedGameApp.DataAccessLayer/Interfaces/IQuestionAccessLayer.cs` - Ajouter GetAllThemesAsync()

#### ✅ Critères d'acceptation

- [ ] Formulaire permet de créer une question avec 4 options
- [ ] Sélection de la bonne réponse (1-4)
- [ ] Dropdown thème avec tous les thèmes
- [ ] Dropdown difficulté (Facile/Moyenne/Difficile)
- [ ] Validation des champs (question min 10 chars, toutes options requises)
- [ ] Message de succès après création
- [ ] Messages d'erreur clairs si validation échoue
- [ ] Formulaire se réinitialise après création réussie

---

### 9️⃣ Export résultats de partie

**ID** : `FEAT-009`
**Priorité** : ⭐⭐⭐ (Moyenne)
**Effort** : 5-6h
**Valeur** : Souvenir, partage, archivage

#### 🎯 Problème

Impossible d'exporter les résultats d'une partie. Pas de rapport PDF/CSV pour garder un souvenir ou partager.

#### ✅ Solution

Bouton "Exporter" avec choix PDF ou CSV, incluant classement, scores, et optionnellement l'historique des rounds.

#### 💻 Implémentation

**Étape 1 : Ajouter package QuestPDF**
```xml
<!-- Directory.Packages.props -->
<PackageVersion Include="QuestPDF" Version="2024.10.0" />
```

**Étape 2 : Créer IExportService**
```csharp
namespace SpeedGameApp.Business.Services.Interfaces;

public interface IExportService
{
    Task<byte[]> ExportToPdfAsync(Guid partyId);
    Task<byte[]> ExportToCsvAsync(Guid partyId);
    Task<byte[]> ExportRoundHistoryToCsvAsync(Guid partyId);
}
```

**Étape 3 : Implémenter ExportService**
```csharp
namespace SpeedGameApp.Business.Services.Implementations;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text;

public sealed class ExportService(
    IPartyRepository partyRepository,
    IRoundHistoryService roundHistoryService) : IExportService
{
    public async Task<byte[]> ExportToPdfAsync(Guid partyId)
    {
        if (!partyRepository.Parties.TryGetValue(partyId, out var party))
            throw new InvalidOperationException("Partie introuvable.");

        var rounds = roundHistoryService.GetRoundHistory(partyId).ToList();
        var sortedTeams = party.Teams.Values.OrderByDescending(t => t.Score).ToList();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Text($"Résultats de la partie : {party.Name}")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Spacing(10);

                        // Classement
                        column.Item().Text("Classement final").FontSize(16).SemiBold();
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(50);
                                columns.RelativeColumn();
                                columns.ConstantColumn(100);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Rang");
                                header.Cell().Element(CellStyle).Text("Équipe");
                                header.Cell().Element(CellStyle).Text("Score");
                            });

                            int rank = 1;
                            foreach (var team in sortedTeams)
                            {
                                table.Cell().Element(CellStyle).Text(rank.ToString());
                                table.Cell().Element(CellStyle).Text(team.Name);
                                table.Cell().Element(CellStyle).Text(team.Score.ToString());
                                rank++;
                            }
                        });

                        // Statistiques
                        column.Item().PaddingTop(20).Text("Statistiques").FontSize(16).SemiBold();
                        column.Item().Text($"Nombre de rounds joués : {rounds.Count}");
                        column.Item().Text($"Nombre d'équipes : {party.Teams.Count}");
                        column.Item().Text($"Score total : {sortedTeams.Sum(t => t.Score)}");

                        // Historique des rounds (optionnel)
                        if (rounds.Any())
                        {
                            column.Item().PaddingTop(20).Text("Historique des rounds").FontSize(16).SemiBold();

                            foreach (var round in rounds)
                            {
                                column.Item().PaddingTop(10).Column(roundColumn =>
                                {
                                    roundColumn.Item().Text($"Round {round.Number} : {round.Question}").SemiBold();
                                    roundColumn.Item().Text($"Type : {round.ResponseType}");

                                    if (round.WinningTeamId.HasValue && party.Teams.TryGetValue(round.WinningTeamId.Value, out var winner))
                                    {
                                        roundColumn.Item().Text($"Gagnant : {winner.Name}").FontColor(Colors.Green.Medium);
                                    }
                                });
                            }
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
            });
        });

        return await Task.Run(() => document.GeneratePdf());
    }

    public async Task<byte[]> ExportToCsvAsync(Guid partyId)
    {
        if (!partyRepository.Parties.TryGetValue(partyId, out var party))
            throw new InvalidOperationException("Partie introuvable.");

        var sortedTeams = party.Teams.Values.OrderByDescending(t => t.Score).ToList();

        var csv = new StringBuilder();
        csv.AppendLine("Rang,Équipe,Score");

        int rank = 1;
        foreach (var team in sortedTeams)
        {
            csv.AppendLine($"{rank},{team.Name},{team.Score}");
            rank++;
        }

        return await Task.Run(() => Encoding.UTF8.GetBytes(csv.ToString()));
    }

    public async Task<byte[]> ExportRoundHistoryToCsvAsync(Guid partyId)
    {
        if (!partyRepository.Parties.TryGetValue(partyId, out var party))
            throw new InvalidOperationException("Partie introuvable.");

        var rounds = roundHistoryService.GetRoundHistory(partyId).ToList();

        var csv = new StringBuilder();
        csv.AppendLine("Round,Question,Type,Équipe,Réponse,Correct,Points,Temps (s)");

        foreach (var round in rounds)
        {
            foreach (var response in round.TeamResponses.Values)
            {
                var timeElapsed = (response.ResponseTime - round.StartTime).TotalSeconds;
                csv.AppendLine($"{round.Number},{EscapeCsv(round.Question)},{round.ResponseType},{EscapeCsv(response.TeamName)},{EscapeCsv(response.Response)},{response.IsCorrect},{response.PointsAwarded},{timeElapsed:F1}");
            }
        }

        return await Task.Run(() => Encoding.UTF8.GetBytes(csv.ToString()));
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
    }
}
```

**Étape 4 : Créer endpoint pour téléchargement**
```csharp
// Dans un nouveau contrôleur ExportController.cs
[ApiController]
[Route("api/[controller]")]
public class ExportController(IExportService exportService) : ControllerBase
{
    [HttpGet("pdf/{partyId}")]
    public async Task<IActionResult> ExportPdf(Guid partyId)
    {
        try
        {
            var pdf = await exportService.ExportToPdfAsync(partyId);
            return File(pdf, "application/pdf", $"partie_{partyId}.pdf");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("csv/{partyId}")]
    public async Task<IActionResult> ExportCsv(Guid partyId)
    {
        try
        {
            var csv = await exportService.ExportToCsvAsync(partyId);
            return File(csv, "text/csv", $"classement_{partyId}.csv");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("csv/history/{partyId}")]
    public async Task<IActionResult> ExportRoundHistoryCsv(Guid partyId)
    {
        try
        {
            var csv = await exportService.ExportRoundHistoryToCsvAsync(partyId);
            return File(csv, "text/csv", $"historique_{partyId}.csv");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
```

**Étape 5 : Ajouter boutons export dans PartyAdmin.razor**
```razor
<div class="card mb-3">
    <div class="card-header">
        <h5>Exporter les résultats</h5>
    </div>
    <div class="card-body">
        <div class="btn-group">
            <a href="/api/export/pdf/@PartyId" class="btn btn-danger" target="_blank">
                <i class="bi bi-file-pdf"></i> Télécharger PDF
            </a>
            <a href="/api/export/csv/@PartyId" class="btn btn-success" target="_blank">
                <i class="bi bi-file-csv"></i> Télécharger Classement CSV
            </a>
            <a href="/api/export/csv/history/@PartyId" class="btn btn-info" target="_blank">
                <i class="bi bi-file-csv"></i> Télécharger Historique CSV
            </a>
        </div>
    </div>
</div>
```

#### 📁 Fichiers à créer/modifier

1. **Ajouter** package `QuestPDF` dans `Directory.Packages.props`
2. **Créer** `SpeedGameApp.Business/Services/Interfaces/IExportService.cs`
3. **Créer** `SpeedGameApp.Business/Services/Implementations/ExportService.cs`
4. **Créer** `SpeedGameApp/Controllers/ExportController.cs`
5. **Modifier** `SpeedGameApp/Pages/PartyAdmin.razor` - Ajouter boutons export
6. **Modifier** `SpeedGameApp.Business/Extensions/BusinessExtensions.cs` - Enregistrer IExportService
7. **Modifier** `SpeedGameApp/Program.cs` - Ajouter services.AddControllers() pour API

#### ✅ Critères d'acceptation

- [ ] Bouton "Télécharger PDF" génère PDF avec classement et stats
- [ ] Bouton "Télécharger CSV" génère CSV du classement
- [ ] Bouton "Télécharger Historique CSV" génère CSV détaillé des rounds
- [ ] PDF inclut : nom partie, classement, stats, historique rounds
- [ ] CSV classement : Rang, Équipe, Score
- [ ] CSV historique : Round, Question, Type, Équipe, Réponse, Correct, Points, Temps
- [ ] Fichiers téléchargés avec noms explicites (partie_guid.pdf, etc.)
- [ ] Gestion erreur si partie introuvable

---

### 🔟 Mode "Final Jeopardy" / Round bonus

**ID** : `FEAT-010`
**Priorité** : ⭐⭐⭐ (Moyenne)
**Effort** : 8-10h
**Valeur** : Dramatisation, suspense, remontadas

#### 🎯 Problème

Pas de round final spectaculaire. Les équipes en retard ne peuvent pas faire de remontada. Manque de climax.

#### ✅ Solution

Mode spécial "Final Round" où les équipes misent des points sur une question difficile, avec multiplicateur x2 ou x3.

#### 💻 Implémentation

**Étape 1 : Ajouter au PartyDto**
```csharp
public sealed class PartyDto
{
    // ... propriétés existantes ...

    public bool IsFinalRound { get; set; }
    public Dictionary<Guid, int> TeamBets { get; set; } = []; // Mise par équipe
    public int FinalRoundMultiplier { get; set; } = 2; // x2 par défaut
}
```

**Étape 2 : Créer IFinalRoundService**
```csharp
namespace SpeedGameApp.Business.Services.Interfaces;

public interface IFinalRoundService
{
    void StartFinalRound(Guid partyId, int multiplier = 2);
    void PlaceBet(Guid partyId, Guid teamId, int betAmount);
    void EndFinalRound(Guid partyId);
    bool IsFinalRoundActive(Guid partyId);
    Dictionary<Guid, int> GetBets(Guid partyId);
}
```

**Étape 3 : Implémenter FinalRoundService**
```csharp
namespace SpeedGameApp.Business.Services.Implementations;

public sealed class FinalRoundService(
    IPartyRepository partyRepository,
    IPartyEventPublisher eventPublisher) : IFinalRoundService
{
    public void StartFinalRound(Guid partyId, int multiplier = 2)
    {
        if (!partyRepository.Parties.TryGetValue(partyId, out var party))
            return;

        party.IsFinalRound = true;
        party.FinalRoundMultiplier = multiplier;
        party.TeamBets.Clear();

        eventPublisher.NotifyPartyChanged(partyId, party);
    }

    public void PlaceBet(Guid partyId, Guid teamId, int betAmount)
    {
        if (!partyRepository.Parties.TryGetValue(partyId, out var party))
            return;

        if (!party.Teams.TryGetValue(teamId, out var team))
            return;

        // Limite : ne peut pas miser plus que son score actuel
        var maxBet = team.Score;
        var actualBet = Math.Min(betAmount, maxBet);

        party.TeamBets[teamId] = actualBet;

        eventPublisher.NotifyPartyChanged(partyId, party);
    }

    public void EndFinalRound(Guid partyId)
    {
        if (!partyRepository.Parties.TryGetValue(partyId, out var party))
            return;

        party.IsFinalRound = false;
        party.TeamBets.Clear();

        eventPublisher.NotifyPartyChanged(partyId, party);
    }

    public bool IsFinalRoundActive(Guid partyId)
    {
        if (!partyRepository.Parties.TryGetValue(partyId, out var party))
            return false;

        return party.IsFinalRound;
    }

    public Dictionary<Guid, int> GetBets(Guid partyId)
    {
        if (!partyRepository.Parties.TryGetValue(partyId, out var party))
            return [];

        return party.TeamBets;
    }
}
```

**Étape 4 : Modifier GameplayService pour gérer mises**
```csharp
public async Task AddPointsAsync(TeamDto teamDto, int points, CancellationToken cancellationToken)
{
    // Si final round, multiplier par la mise
    if (partyRepository.Parties.TryGetValue(teamDto.PartyId, out var party) && party.IsFinalRound)
    {
        if (party.TeamBets.TryGetValue(teamDto.Id, out var bet))
        {
            var multipliedPoints = points > 0
                ? bet * party.FinalRoundMultiplier  // Bonne réponse : gain = mise x multiplicateur
                : -bet;  // Mauvaise réponse : perd la mise

            stateManager.AddPoints(teamDto, multipliedPoints);
        }
    }
    else
    {
        stateManager.AddPoints(teamDto, points);
    }

    await partyManagementService.SavePartyAsync(teamDto.PartyId, cancellationToken);
}
```

**Étape 5 : UI Admin - PartyAdmin.razor**
```razor
@if (!CurrentParty.IsFinalRound)
{
    <div class="card mb-3 border-warning">
        <div class="card-header bg-warning text-dark">
            <h5>🏆 Round Final (Final Jeopardy)</h5>
        </div>
        <div class="card-body">
            <p>Lancez le round final ! Les équipes vont miser des points sur une question difficile.</p>
            <div class="mb-3">
                <label class="form-label">Multiplicateur de points</label>
                <select class="form-select" @bind="finalMultiplier">
                    <option value="2">x2 (Double)</option>
                    <option value="3">x3 (Triple)</option>
                    <option value="5">x5 (Quintuple)</option>
                </select>
            </div>
            <button class="btn btn-warning btn-lg" @onclick="StartFinalRound">
                🏆 Lancer le Round Final
            </button>
        </div>
    </div>
}
else
{
    <div class="card mb-3 border-success">
        <div class="card-header bg-success text-white">
            <h5>🏆 Round Final en cours (Multiplicateur x@CurrentParty.FinalRoundMultiplier)</h5>
        </div>
        <div class="card-body">
            <h6>Mises des équipes :</h6>
            <ul class="list-group mb-3">
                @foreach (var team in CurrentParty.Teams.Values)
                {
                    var bet = CurrentParty.TeamBets.TryGetValue(team.Id, out var b) ? b : 0;
                    <li class="list-group-item d-flex justify-content-between align-items-center">
                        @team.Name (Score : @team.Score)
                        <span class="badge bg-primary rounded-pill">Mise : @bet pts</span>
                    </li>
                }
            </ul>

            <p class="text-muted">
                <i class="bi bi-info-circle"></i> Les équipes peuvent maintenant placer leurs mises.
                Bonne réponse = Mise x @CurrentParty.FinalRoundMultiplier.
                Mauvaise réponse = Perte de la mise.
            </p>

            <button class="btn btn-danger" @onclick="EndFinalRound">
                Terminer le Round Final
            </button>
        </div>
    </div>
}
```

**Étape 6 : UI Équipe - PartyTeamPlay.razor**
```razor
@if (CurrentParty.IsFinalRound && !hasBetPlaced)
{
    <div class="alert alert-warning">
        <h4>🏆 ROUND FINAL !</h4>
        <p>Placez votre mise (maximum : @CurrentTeam.Score points)</p>
        <div class="input-group mb-3">
            <input type="number"
                   class="form-control"
                   min="0"
                   max="@CurrentTeam.Score"
                   @bind="betAmount"
                   placeholder="Montant de la mise" />
            <button class="btn btn-warning" @onclick="PlaceBet">
                Miser @betAmount points
            </button>
        </div>
        <p class="text-muted">
            Bonne réponse : +@(betAmount * CurrentParty.FinalRoundMultiplier) points<br/>
            Mauvaise réponse : -@betAmount points
        </p>
    </div>
}
else if (CurrentParty.IsFinalRound && hasBetPlaced)
{
    <div class="alert alert-success">
        ✅ Mise placée : @betAmount points<br/>
        En attente de la question finale...
    </div>
}
```

**Étape 7 : PartyTeamPlay.razor.cs**
```csharp
[Inject]
public IFinalRoundService FinalRoundService { get; set; } = default!;

private int betAmount = 0;
private bool hasBetPlaced = false;

protected override async Task OnParametersSetAsync()
{
    await base.OnParametersSetAsync();

    if (CurrentParty.IsFinalRound)
    {
        var bets = FinalRoundService.GetBets(PartyId);
        hasBetPlaced = bets.ContainsKey(CurrentTeam.Id);
        if (hasBetPlaced)
            betAmount = bets[CurrentTeam.Id];
    }
}

private void PlaceBet()
{
    FinalRoundService.PlaceBet(PartyId, CurrentTeam.Id, betAmount);
    hasBetPlaced = true;
}
```

#### 📁 Fichiers à créer/modifier

1. **Créer** `SpeedGameApp.Business/Services/Interfaces/IFinalRoundService.cs`
2. **Créer** `SpeedGameApp.Business/Services/Implementations/FinalRoundService.cs`
3. **Modifier** `SpeedGameApp.Business/Data/PartyDto.cs` - Ajouter IsFinalRound, TeamBets, FinalRoundMultiplier
4. **Modifier** `SpeedGameApp.Business/Services/Implementations/GameplayService.cs` - Gérer mises et multiplicateurs
5. **Modifier** `SpeedGameApp/Pages/PartyAdmin.razor` - UI admin final round
6. **Modifier** `SpeedGameApp/Pages/PartyAdmin.razor.cs` - Logique final round
7. **Modifier** `SpeedGameApp/Pages/Game/PartyTeamPlay.razor` - UI équipe placement mise
8. **Modifier** `SpeedGameApp/Pages/Game/PartyTeamPlay.razor.cs` - Logique placement mise
9. **Modifier** `SpeedGameApp/Pages/GamePageBase.cs` - Injecter IFinalRoundService
10. **Modifier** `SpeedGameApp.Business/Extensions/BusinessExtensions.cs` - Enregistrer IFinalRoundService

#### ✅ Critères d'acceptation

- [ ] Admin peut lancer "Round Final" avec multiplicateur choisi (x2, x3, x5)
- [ ] Les équipes voient alerte "Round Final" et peuvent placer leur mise
- [ ] Mise limitée au score actuel de l'équipe
- [ ] Calcul prévisualisé : bonne réponse = mise x multiplicateur, mauvaise = -mise
- [ ] Admin voit toutes les mises placées en temps réel
- [ ] Bonne réponse : score += mise * multiplicateur
- [ ] Mauvaise réponse : score -= mise
- [ ] Admin peut terminer le round final
- [ ] Remontadas spectaculaires possibles !

---

## 🌟 BASSE PRIORITÉ (Nice to have)

### 1️⃣1️⃣ Statistiques globales

**ID** : `FEAT-011`
**Priorité** : ⭐⭐ (Basse)
**Effort** : 6-8h
**Valeur** : Fun, compétitif, engagement long terme

#### 🎯 Problème

Aucune mémoire entre les parties. Impossible de savoir quelle équipe gagne le plus, quelles questions sont difficiles, etc.

#### ✅ Solution

Page `/admin/stats` avec statistiques agrégées :
- Top 10 équipes all-time (par victoires, par score total)
- Questions les plus ratées
- Thèmes les plus choisis
- Nombre de parties jouées
- Graphiques de performance

#### 💻 Implémentation

**Concepts clés** :
- Ajouter table `GameHistory` en DB avec PartyId, Winner, FinalScores, DatePlayed
- Service `IStatisticsService` pour requêtes agrégées
- Utiliser Chart.js ou ApexCharts pour graphiques Blazor
- Cache des stats (rafraîchir toutes les heures)

**Fichiers à créer** :
- `IStatisticsService.cs`
- `StatisticsService.cs`
- `Pages/Admin/Statistics.razor`
- Migration EF Core pour table GameHistory

#### ✅ Critères d'acceptation

- [ ] Top 10 équipes par victoires
- [ ] Top 10 équipes par score total
- [ ] Top 10 questions les plus ratées (% échec)
- [ ] Graphique : Parties jouées par semaine/mois
- [ ] Graphique : Distribution des scores
- [ ] Thèmes les plus choisis (classement)

---

### 1️⃣2️⃣ Mode "Audience/Spectateur"

**ID** : `FEAT-012`
**Priorité** : ⭐⭐ (Basse)
**Effort** : 2-3h
**Valeur** : Ambiance, soirées avec public

#### 🎯 Problème

Impossible de projeter le jeu sur un écran pour spectateurs. Il faudrait partager l'écran admin, mais ça expose les contrôles.

#### ✅ Solution

URL spéciale `/party/{id}/spectate` en lecture seule affichant :
- Leaderboard en temps réel
- Question actuelle
- Réponses des équipes en live
- Pas de boutons de contrôle

#### 💻 Implémentation

**Étape 1 : Créer SpectatorView.razor**
```razor
@page "/party/{PartyId:guid}/spectate"
@inherits PartyPageBase

<div class="spectator-view">
    <!-- Grand écran leaderboard -->
    <div class="leaderboard-large">
        <h1>@CurrentParty.Name</h1>
        <table class="table table-dark table-striped">
            <thead>
                <tr>
                    <th>Rang</th>
                    <th>Équipe</th>
                    <th>Score</th>
                </tr>
            </thead>
            <tbody>
                @{ int rank = 1; }
                @foreach (var team in CurrentParty.Teams.Values.OrderByDescending(t => t.Score))
                {
                    <tr class="@(rank == 1 ? "table-warning" : "")">
                        <td>@rank</td>
                        <td>@team.Name</td>
                        <td>@team.Score</td>
                    </tr>
                    rank++;
                }
            </tbody>
        </table>
    </div>

    <!-- Question actuelle -->
    @if (CurrentParty.CurrentQcm != null)
    {
        <div class="current-question">
            <h3>Question : @CurrentParty.CurrentQcm.Question</h3>
            <!-- Options cachées ou visibles selon config -->
        </div>
    }
</div>

<style>
    .spectator-view {
        background: #1a1a1a;
        color: white;
        min-height: 100vh;
        padding: 2rem;
    }

    .leaderboard-large h1 {
        font-size: 4rem;
        text-align: center;
        margin-bottom: 2rem;
    }

    .leaderboard-large table {
        font-size: 2rem;
    }

    .current-question {
        margin-top: 3rem;
        font-size: 2.5rem;
        text-align: center;
    }
</style>
```

**Fichiers à créer** :
- `Pages/SpectatorView.razor`
- `Pages/SpectatorView.razor.cs`

#### ✅ Critères d'acceptation

- [ ] URL `/party/{id}/spectate` accessible
- [ ] Leaderboard grand format (police 2-4rem)
- [ ] Mise à jour automatique en temps réel
- [ ] Aucun bouton de contrôle visible
- [ ] Thème sombre pour projection (fond noir, texte blanc)
- [ ] Question actuelle affichée en grand

---

### 1️⃣3️⃣ Templates de parties

**ID** : `FEAT-013`
**Priorité** : ⭐⭐ (Basse)
**Effort** : 6-8h
**Valeur** : Gain de temps pour parties récurrentes

#### 🎯 Problème

Recréer manuellement la configuration pour des parties similaires (mêmes thèmes, mêmes paramètres). Répétitif.

#### ✅ Solution

Système de templates :
- Sauvegarder configuration actuelle comme template
- Charger template pour créer nouvelle partie
- Templates incluent : thèmes, timer par défaut, multiplicateur final round, etc.

#### 💻 Implémentation

**Concepts** :
- Table `PartyTemplate` avec JSON de configuration
- Service `ITemplateService`
- Page `/admin/templates` pour CRUD templates
- Dropdown "Charger template" sur création de partie

**Fichiers à créer** :
- `SpeedGameApp.DataAccessLayer/Entities/PartyTemplate.cs`
- `ITemplateService.cs`
- `TemplateService.cs`
- `Pages/Admin/Templates.razor`
- Migration EF Core

#### ✅ Critères d'acceptation

- [ ] Bouton "Sauvegarder comme template" dans PartyAdmin
- [ ] Nom de template personnalisable
- [ ] Liste templates dans `/admin/templates`
- [ ] Bouton "Charger template" sur Index.razor
- [ ] Template inclut : thèmes, timer, multiplicateur
- [ ] CRUD complet sur templates (modifier, supprimer)

---

### 1️⃣4️⃣ Système de manches

**ID** : `FEAT-014`
**Priorité** : ⭐⭐ (Basse)
**Effort** : 8-10h
**Valeur** : Structure le jeu, variété

#### 🎯 Problème

Le jeu est continu sans structure en manches. Pas de progression claire (manche 1, 2, 3).

#### ✅ Solution

Système de manches configurables :
- Admin définit 3 manches : ex "Manche 1 : Buzzer", "Manche 2 : QCM", "Manche 3 : Proposition"
- Changement automatique de mode entre manches
- Scores intermédiaires visibles
- Pause entre manches

#### 💻 Implémentation

**Étape 1 : Modèle Round (différent de RoundDto historique)**
```csharp
public record RoundConfig(
    int Number,
    string Name,
    ResponseType ResponseType,
    int QuestionCount,
    int TimeoutSeconds
);

public sealed class PartyDto
{
    public List<RoundConfig> RoundConfigs { get; set; } = [];
    public int CurrentRoundIndex { get; set; } = 0;
    public RoundConfig? CurrentRound => CurrentRoundIndex < RoundConfigs.Count ? RoundConfigs[CurrentRoundIndex] : null;
}
```

**Étape 2 : IRoundManagementService**
```csharp
public interface IRoundManagementService
{
    void ConfigureRounds(Guid partyId, List<RoundConfig> rounds);
    void StartNextRound(Guid partyId);
    void EndCurrentRound(Guid partyId);
    RoundConfig? GetCurrentRound(Guid partyId);
    int GetCurrentRoundNumber(Guid partyId);
}
```

**Fichiers à créer** :
- `RoundConfig.cs`
- `IRoundManagementService.cs`
- `RoundManagementService.cs`
- UI configuration manches dans PartyAdmin
- Indicateur "Manche X/Y" dans UI

#### ✅ Critères d'acceptation

- [ ] Admin configure 2-5 manches avec nom, type, nb questions
- [ ] Indicateur visible "Manche 2/3" dans UI équipe
- [ ] Bouton "Manche suivante" pour admin
- [ ] Changement automatique de ResponseType selon la manche
- [ ] Scores intermédiaires sauvegardés par manche
- [ ] Pause configurable entre manches (countdown 10s)

---

### 1️⃣5️⃣ Sons et effets visuels

**ID** : `FEAT-015`
**Priorité** : ⭐ (Très basse)
**Effort** : 4-6h
**Valeur** : Ambiance, ludique, mais non essentiel

#### 🎯 Problème

L'interface est silencieuse et statique. Pas d'effets sonores ou visuels pour dynamiser.

#### ✅ Solution

Ajout d'effets audio/visuels :
- Son de buzzer quand équipe buzze
- Animation de confettis pour bonne réponse
- Countdown sonore (bip dernières 5 secondes)
- Animation de victoire à la fin

#### 💻 Implémentation

**Étape 1 : Ajouter fichiers audio**
```
wwwroot/sounds/
  ├── buzzer.mp3
  ├── correct.mp3
  ├── wrong.mp3
  ├── tick.mp3
  └── victory.mp3
```

**Étape 2 : Service ISoundService**
```csharp
public interface ISoundService
{
    Task PlayBuzzerAsync();
    Task PlayCorrectAsync();
    Task PlayWrongAsync();
    Task PlayTickAsync();
    Task PlayVictoryAsync();
}
```

**Étape 3 : JSInterop pour audio**
```javascript
// wwwroot/js/sounds.js
window.playSound = function(soundFile) {
    const audio = new Audio(`/sounds/${soundFile}`);
    audio.play();
};
```

**Étape 4 : Confettis avec canvas-confetti**
```razor
@inject IJSRuntime JS

<button @onclick="ShowConfetti">Gagné !</button>

@code {
    private async Task ShowConfetti()
    {
        await JS.InvokeVoidAsync("confetti", new
        {
            particleCount = 100,
            spread = 70,
            origin = new { y = 0.6 }
        });
    }
}
```

**Fichiers à créer/modifier** :
- `wwwroot/sounds/*.mp3`
- `wwwroot/js/sounds.js`
- `ISoundService.cs`
- `SoundService.cs` (avec JSInterop)
- Intégrer dans Buzzer.razor, QCM.razor, PartyAdmin.razor

#### ✅ Critères d'acceptation

- [ ] Son de buzzer au clic
- [ ] Son correct/incorrect après validation QCM
- [ ] Tick sonore dernières 5 secondes du timer
- [ ] Confettis animés pour bonne réponse
- [ ] Animation victory à la fin de partie
- [ ] Option pour couper les sons (toggle dans settings)

---

## Index par catégorie

### 🔴 UX / Interface utilisateur
- FEAT-001 : Affichage erreurs utilisateur
- FEAT-002 : Confirmations actions destructives
- FEAT-004 : Indicateurs de chargement
- FEAT-012 : Mode spectateur
- FEAT-015 : Sons et effets visuels

### 🟢 Gameplay / Mécanique de jeu
- FEAT-005 : Historique des réponses
- FEAT-006 : Timer par question
- FEAT-010 : Round final bonus
- FEAT-014 : Système de manches

### 🔵 Gestion de contenu
- FEAT-007 : Filtrage questions QCM
- FEAT-008 : Création questions via UI
- FEAT-013 : Templates de parties

### 🟡 Analyse / Export
- FEAT-003 : Auto-save scores (déjà fait)
- FEAT-009 : Export résultats
- FEAT-011 : Statistiques globales

---

## Priorisation recommandée (Quick Wins first)

### Phase 1 - Quick Wins (1-2 semaines) ✅
1. FEAT-001 : Affichage erreurs (1-2h)
2. FEAT-002 : Confirmations (2-3h)
3. FEAT-004 : Loading indicators (2-3h)
4. FEAT-012 : Mode spectateur (2-3h)

**Total Phase 1 : ~10h**

### Phase 2 - Game Changers (2-3 semaines) 🚀
5. FEAT-005 : Historique rounds (4-6h)
6. FEAT-006 : Timer questions (6-8h)
7. FEAT-007 : Filtrage QCM (3-4h)
8. FEAT-008 : Création questions UI (4-5h)

**Total Phase 2 : ~20h**

### Phase 3 - Advanced Features (3-4 semaines) 🎯
9. FEAT-009 : Export résultats (5-6h)
10. FEAT-010 : Round final (8-10h)
11. FEAT-011 : Statistiques globales (6-8h)

**Total Phase 3 : ~22h**

### Phase 4 - Polish (optionnel) ✨
12. FEAT-013 : Templates (6-8h)
13. FEAT-014 : Système manches (8-10h)
14. FEAT-015 : Sons et effets (4-6h)

**Total Phase 4 : ~20h**

---

## Conclusion

Ce document recense **15 améliorations fonctionnelles** pour SpeedGameApp, classées par priorité et avec implémentations détaillées.

**Prochaines étapes** :
1. Valider les priorités avec l'équipe
2. Choisir 1-2 features pour démarrer
3. Créer branches Git par feature
4. Implémenter, tester, déployer
5. Itérer !

**Contact** : Pour toute question sur une feature spécifique, référencer le code `FEAT-XXX` correspondant.

---

*Dernière mise à jour : 2026-01-02*
