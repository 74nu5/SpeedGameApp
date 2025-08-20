# Utilisation de la classe PartyContext

## Vue d'ensemble

La classe `PartyContext` est un composant central du projet SpeedGameApp qui agit comme un gestionnaire d'état en mémoire pour toutes les parties de jeu actives. Elle fonctionne comme un singleton et coordonne les interactions entre les équipes, les parties et les réponses des joueurs.

## Architecture et Design

### Pattern Singleton
```csharp
services.TryAddSingleton<PartyContext>();
```
La classe est enregistrée comme singleton dans le conteneur d'injection de dépendances, garantissant qu'une seule instance existe pour toute l'application.

### Stockage Thread-Safe
```csharp
private readonly IDictionary<Guid, PartyDto> parties = new ConcurrentDictionary<Guid, PartyDto>();
```
Utilise `ConcurrentDictionary` pour assurer la thread-safety lors des accès concurrents aux données des parties.

## Fonctionnalités principales

### 1. Gestion des Parties
- **Création** : Ajouter de nouvelles parties via `LoadParty()`
- **Suppression** : Supprimer une partie spécifique ou toutes les parties
- **Vérification d'existence** : `ExistsParty(Guid id)`
- **Accès en lecture seule** : Propriété `Parties` retournant un `IReadOnlyDictionary`

### 2. Gestion des Équipes
- **Ajout de points** : `AddPoints(TeamDto teamDto, int points)`
- **Buzz des équipes** : `BuzzTeam(Guid partyId, Guid teamId)`
- **Propositions** : `PropositionTeam(Guid partyId, Guid teamId, string response)`
- **Suppression d'équipes** : `RemoveTeam(Guid partyId, Guid teamId)`

### 3. Gestion des Réponses
- **Types de réponses** : `SetCurrentResponse(Guid partyId, ResponseType responseType)`
  - None, Buzzer, Proposition, Qcm
- **Reset des réponses** : `ResetTeam(Guid partyId)` remet à zéro les buzz et réponses

### 4. Système d'Événements
```csharp
public event EventHandler<PartyDto>? PartyChanged;
```
Notifie les composants de l'interface utilisateur des changements d'état via des événements.

## Intégration avec GameService

Le `GameService` utilise `PartyContext` comme couche d'abstraction :

```csharp
public sealed class GameService
{
    private readonly PartyContext context;
    
    public event EventHandler<PartyDto>? PartyChanged
    {
        add => this.context.PartyChanged += value;
        remove => this.context.PartyChanged -= value;
    }
}
```

## Flux de données typique

1. **Création d'une partie** → `GameService.CreatePartyAsync()` → `PartyContext.LoadParty()`
2. **Action d'équipe** (buzz, réponse) → `PartyContext.BuzzTeam()` ou `PropositionTeam()`
3. **Notification** → Événement `PartyChanged` déclenché
4. **Mise à jour UI** → Les composants Blazor réagissent aux changements d'état

## Avantages de cette architecture

- **Centralisation** : Point unique de vérité pour l'état des parties
- **Réactivité** : Système d'événements pour les mises à jour en temps réel
- **Thread-safety** : Gestion sécurisée des accès concurrents
- **Séparation des responsabilités** : Logique métier séparée de la persistance

## Utilisation dans l'interface utilisateur

Les pages Blazor héritent de `PartyPageBase` qui s'abonne aux événements :
```csharp
this.CurrentParty.PartyChanged += async (_, _) => await this.InvokeAsync(this.StateHasChanged).ConfigureAwait(true);
```

Cette classe constitue le cœur de la gestion d'état de l'application de jeu de vitesse, permettant une synchronisation en temps réel entre les différents clients connectés.

---

*Note : Cette analyse est basée sur les résultats de recherche disponibles qui peuvent être incomplets. Pour une vue complète, consultez [la recherche GitHub](https://github.com/74nu5/SpeedGameApp/search?q=PartyContext) du projet.*