# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

SpeedGameApp is a C# Blazor Server application for hosting speed quiz games (QCM questions and blind tests). The project uses .NET 10.0 and follows a clean layered architecture with strict adherence to SOLID principles, particularly the Single Responsibility Principle.

## Build and Development Commands

### Building the solution
```bash
dotnet build SpeedGameApp.sln
```

### Running the application
```bash
dotnet run --project SpeedGameApp/SpeedGameApp.csproj
```

### Database
- **Development**: Uses SQLite (`SpeedGameApp.db` in the main project folder)
- **Production**: Uses SQL Server (connection string from `appsettings.Production.json`)
- SQL scripts are located in `sql/` directory

### Code Quality
The project enforces StyleCop rules via `stylecop.json` and uses `.editorconfig` for consistent code formatting. XML documentation is required for all public APIs.

## Architecture

### Layer Structure

The solution is organized into four projects following a strict layered architecture:

1. **SpeedGameApp** (Presentation Layer)
   - Blazor Server application
   - Razor components in `Pages/` directory:
     - `Pages/Game/` - Team gameplay pages (creation, themes, play)
     - `Pages/Admin/` - Admin pages for managing parties and questions
   - Base classes: `GamePageBase.cs`, `PartyPageBase.cs`
   - Entry point: `Program.cs` uses `builder.Services.AddBusinessServices()` for DI setup

2. **SpeedGameApp.Business** (Business Logic Layer)
   - Organized into specialized services following SRP
   - **Services** (`Services/Implementations/` and `Services/Interfaces/`):
     - `IPartyManagementService` - Party lifecycle management
     - `IGameplayService` - Buzz and gameplay mechanics
     - `IQcmService` - QCM question handling
     - `IThemeService` - Blind test theme management
   - **Infrastructure services** (in-memory state management):
     - `IPartyRepository` - In-memory party storage (ConcurrentDictionary)
     - `IPartyStateManager` - Game state (buzz, responses, scores)
     - `IPartyEventPublisher` - Event publishing for party changes
     - `IThemeManager` - Theme selection and choices
   - **Validators** (`Validators/`) - FluentValidation validators
   - **Data** (`Data/`) - DTOs and business models
   - **Common** (`Common/`) - Shared utilities
   - `CsvService.cs` - CSV export functionality

3. **SpeedGameApp.DataAccessLayer** (Data Access Layer)
   - Entity Framework Core with `SpeedGameDbContext`
   - **Entities** (`Entities/`): `Party`, `Team`, `QcmQuestion`, `QcmTheme`, `Theme`
   - **Access Layers** (`AccessLayers/`) with interfaces (`Interfaces/`):
     - `IPartyAccessLayer` / `PartyAccessLayer`
     - `IQuestionAccessLayer` / `QuestionAccessLayer`
     - `IThemeAccessLayer` / `ThemeAccessLayer`
   - Database configured via `DalExtensions.AddDalServices()`

4. **SpeedGameApp.DataEnum** (Shared Enumerations)
   - `Difficulty` - Question difficulty levels
   - `ResponseType` - Types of responses (Buzzer, Text, etc.)

### Dependency Flow

```
SpeedGameApp (Presentation)
    ↓
SpeedGameApp.Business (Business Logic)
    ├─→ Specialized Services (IPartyManagementService, IGameplayService, etc.)
    └─→ Infrastructure Services (IPartyRepository, IPartyStateManager, etc.)
        ↓
SpeedGameApp.DataAccessLayer (Data Access)
    └─→ Access Layers (IPartyAccessLayer, IQuestionAccessLayer, etc.)
        ↓
    SpeedGameDbContext → Database

SpeedGameApp.DataEnum is referenced by all layers.
```

### Important Architectural Principles

**Single Responsibility Principle (SRP) is paramount**:
- The legacy `PartyContext` (257 lines) was refactored into 4 specialized services (see `PARTYCONTEXT_REFACTORING.md`)
- Each service has exactly one reason to change
- Always prefer creating new focused services over expanding existing ones

**Dependency Injection**:
- All services are registered in `BusinessExtensions.AddBusinessServices()` and `DalExtensions.AddDalServices()`
- Infrastructure services are **Singleton** (PartyRepository, StateManager, EventPublisher, ThemeManager)
- Business services are **Transient** (PartyManagementService, GameplayService, etc.)
- Access layers are **Scoped** (tied to DbContext lifetime)

**Interface-based design**:
- All services and access layers have corresponding interfaces
- Always inject interfaces, never concrete implementations
- This enables mocking for tests and follows Dependency Inversion Principle

## Key Patterns and Conventions

### Service Design

When creating or modifying services:
1. **One responsibility per service** - If a service does multiple things, split it
2. **Interface first** - Define the interface in `Services/Interfaces/`, implement in `Services/Implementations/`
3. **Constructor injection** - Use primary constructors: `public sealed class MyService(IDependency dep)`
4. **Async all the way** - Database operations use async/await with `CancellationToken`
5. **Validation** - Use FluentValidation for complex validation logic

### Naming Conventions

- **Services**: Suffix with `Service` (e.g., `GameplayService`)
- **Access Layers**: Suffix with `AccessLayer` (e.g., `PartyAccessLayer`)
- **Repositories**: Suffix with `Repository` (e.g., `PartyRepository`)
- **DTOs**: Suffix with `Dto` (e.g., `PartyDto`, `TeamDto`)
- **Entities**: No suffix, match database table names (e.g., `Party`, `Team`)

### State Management

The application uses a hybrid state management approach:
- **In-memory state** (via `IPartyRepository`) - Active game sessions stored in `ConcurrentDictionary<Guid, PartyDto>`
- **Persistent state** (via Access Layers) - Party metadata, teams, questions, themes in database
- **Events** (via `IPartyEventPublisher`) - `PartyChanged` event notifies Blazor components of state updates

When state changes:
1. Update in-memory state via `IPartyStateManager` or `IPartyRepository`
2. Persist to database via appropriate Access Layer
3. Publish event via `IPartyEventPublisher` to refresh UI

## Package Management

Uses **Central Package Management** via `Directory.Packages.props`:
- All NuGet package versions defined centrally
- Projects reference packages without version numbers
- Key packages:
  - `Microsoft.EntityFrameworkCore` (10.0.0)
  - `Microsoft.AspNetCore.Mvc.Testing` (10.0.0)
  - `FluentValidation.DependencyInjectionExtensions` (11.11.0)
  - `StyleCop.Analyzers` (1.2.0-beta.556)

## Common Development Tasks

### Adding a new game feature

1. Identify which layer needs changes (usually Business)
2. Create/modify service interface in `SpeedGameApp.Business/Services/Interfaces/`
3. Implement in `SpeedGameApp.Business/Services/Implementations/`
4. Register in `BusinessExtensions.AddBusinessServices()` with appropriate lifetime
5. If database access needed, add methods to relevant Access Layer interface
6. Update Blazor components to inject and use the new service

### Adding a new entity

1. Create entity class in `SpeedGameApp.DataAccessLayer/Entities/`
2. Add `DbSet<TEntity>` property to `SpeedGameDbContext`
3. Create/update Access Layer to expose CRUD operations
4. Create corresponding DTO in `SpeedGameApp.Business/Data/`
5. Add mapping logic between Entity and DTO

### Modifying party state

Party state is managed by specialized infrastructure services:
- **Adding/removing parties or teams** → `IPartyRepository`
- **Game actions (buzz, responses, scores)** → `IPartyStateManager`
- **Theme selection** → `IThemeManager`
- **Notifying UI of changes** → `IPartyEventPublisher`

Never modify `PartyDto` properties directly from components - always use the appropriate service.

## Documentation Standards

- All public types, members, and methods require XML documentation (`///`)
- Use `<summary>`, `<param>`, `<returns>` tags
- XML documentation is generated to `{AssemblyName}.xml` files (enabled in all `.csproj` files)
- Keep documentation concise but meaningful

## References

- `ARCHITECTURE.md` - High-level architecture overview (in French)
- `PARTYCONTEXT_REFACTORING.md` - Detailed explanation of the SRP refactoring (in French)
- `ARCHITECTURE_RECOMMENDATIONS.md` - Future improvements and best practices (in French)
- `FEATURE_IMPROVEMENTS.md` - Feature enhancement backlog (in French)
