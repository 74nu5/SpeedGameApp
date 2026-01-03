
# Directives Copilot – SpeedGameApp

## Migration vers .NET 9

- **Prompt 1** : Mets à jour tous les fichiers `.csproj` pour cibler `<TargetFramework>net9.0</TargetFramework>`.
- **Prompt 2** : Mets à jour (ou crée) le fichier `global.json` pour spécifier le SDK .NET 9.
- **Prompt 3** : Revois le code pour adopter les nouveautés de C# 13 :  
  - Utilise les `params` collections dans les méthodes appropriées.  
  - Remplace les propriétés automatiques pour utiliser le mot-clé `field`.  
  - Implémente les améliorations du mot-clé `lock`.
- **Prompt 4** : Remplace les usages de JSON par `System.Text.Json` optimisé de .NET 9.
- **Prompt 5** : Utilise les dernières APIs `Span<T>` et `Memory<T>` lorsqu’elles apportent des gains de performance.
- **Prompt 6** : Adapte la configuration ASP.NET Core pour utiliser les middlewares et optimisations d’ASP.NET Core 9.
- **Prompt 7** : Si Blazor est utilisé, applique les nouveautés de Blazor Server-Side de .NET 9.

## Améliorations de conception

- **Prompt 8** : Refactore la classe `PartyContext` pour appliquer le principe de responsabilité unique :  
  - Crée un service `IPartyStateManager` pour la gestion d’état.  
  - Crée un service `IPartyEventPublisher` pour la gestion des événements.  
  - Crée un service `IPartyRepository` pour le stockage en mémoire.
- **Prompt 9** : Crée des interfaces pour tous les services, notamment pour `PartyAccessLayer` :  
  - Génère une interface `IPartyAccessLayer` et modifie la classe pour l’implémenter.
- **Prompt 10** : Renomme la classe `AppContext` de l’accès aux données en `SpeedGameDbContext` pour plus de clarté.
- **Prompt 11** : Revois la gestion d’état dans `PartyContext` afin de la rendre plus modulaire et testable.

---

Ce fichier peut être nommé `CopilotPrompts.md` et déposé à la racine du projet.  
Pour plus de détails ou de prompts, consulte la [revue complète](https://github.com/74nu5/SpeedGameApp/blob/main/RevueByCopilot.md) et les fichiers d’architecture du projet.