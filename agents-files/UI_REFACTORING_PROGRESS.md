# UI Refactoring Progress - Tailwind CSS & Dynamic Themes

**Branch**: `feature/ui-refactoring-tailwind-themes`
**Start Date**: 2026-01-03
**Objective**: Complete UI overhaul with Tailwind CSS and dynamic party-specific theming system

---

## Project Goals

1. **Migrate from Bootstrap 5 to Tailwind CSS**
2. **Create dynamic theming system** - Each party can have its own visual theme
3. **Implement 2 initial themes**:
   - **Cyberpunk 2077**: Neon colors, futuristic, high-tech aesthetic
   - **3bstudio**: Professional, modern, black/blue/yellow palette

---

## Architecture Overview

### Theme System Design

```
SpeedGameApp.DataEnum/
├── PartyTheme.cs (enum: Default, Cyberpunk2077, ThreeBStudio)

SpeedGameApp.Business/
├── Data/
│   └── PartyDto.cs (add Theme property)
├── Services/Interfaces/
│   └── IThemeStyleService.cs (new service for theme CSS generation)
└── Services/Implementations/
    └── ThemeStyleService.cs

SpeedGameApp/
├── wwwroot/
│   ├── css/
│   │   ├── tailwind.css (input file)
│   │   └── app.css (generated output)
│   └── themes/
│       ├── cyberpunk2077.json (theme config)
│       └── 3bstudio.json (theme config)
└── Shared/
    ├── DesignSystem/
    │   ├── Components/
    │   │   ├── Buttons/
    │   │   ├── Cards/
    │   │   ├── Forms/
    │   │   └── Modals/
    │   └── ThemeProvider.razor (injects theme CSS dynamically)
    └── Layouts/
        ├── MainLayout.razor (refactored with Tailwind)
        └── PlayerLayout.razor (refactored with Tailwind)
```

---

## Theme Specifications

### Theme 1: Cyberpunk 2077

**Inspiration**: Cyberpunk 2077 game aesthetic - neon, futuristic, dystopian tech

**Color Palette**:
- **Primary**: Electric Blue (#00F0FF) - Neon cyan
- **Secondary**: Hot Pink/Magenta (#FF00FF) - Neon pink
- **Accent**: Yellow (#FFFF00) - Warning yellow
- **Background Dark**: Deep Black (#0A0A0A) - Almost pure black
- **Background Mid**: Dark Gray (#1A1A1A) - Card backgrounds
- **Text Primary**: Bright Cyan (#00FFFF)
- **Text Secondary**: Light Gray (#B0B0B0)
- **Success**: Neon Green (#00FF41)
- **Danger**: Hot Red (#FF003C)
- **Warning**: Electric Yellow (#FFD700)

**Typography**:
- **Headings**: "Orbitron" or "Rajdhani" (futuristic, tech fonts)
- **Body**: "Roboto Mono" or "Share Tech Mono" (monospace, tech aesthetic)
- **Sizes**: Large, bold headings with glowing text effects

**Visual Elements**:
- Glowing borders (box-shadow with neon colors)
- Scanline effects (subtle CSS animation)
- Glitch effects on hover
- Angular shapes, sharp corners
- Neon glow on buttons and cards
- Dark overlays with grid patterns
- Holographic gradients

**Animations**:
- Pulse effects for interactive elements
- Glitch transitions
- Neon glow animations
- Digital screen flicker

---

### Theme 2: 3bstudio

**Inspiration**: Professional, modern, corporate aesthetic from https://3bstudio.fr

**Color Palette**:
- **Primary**: Deep Blue (#1E3A8A) - Professional blue
- **Secondary**: Black (#000000) - Corporate black
- **Accent**: Golden Yellow (#FBBF24) - Vibrant yellow
- **Background Light**: White (#FFFFFF)
- **Background Mid**: Light Gray (#F3F4F6)
- **Text Primary**: Black (#000000)
- **Text Secondary**: Dark Gray (#4B5563)
- **Success**: Green (#10B981)
- **Danger**: Red (#EF4444)
- **Info**: Sky Blue (#0EA5E9)

**Typography**:
- **Headings**: "Inter" or "Poppins" (modern, clean sans-serif)
- **Body**: "Inter" (consistent, professional)
- **Sizes**: Clear hierarchy with medium weights

**Visual Elements**:
- Clean, rounded corners (border-radius: 0.5rem)
- Subtle shadows (box-shadow: soft)
- Wave decorations (SVG waves like footer)
- Icon-driven design
- Card-based layouts
- Gradient accents (blue to dark blue)
- Yellow highlights for CTAs

**Animations**:
- Smooth transitions (ease-in-out)
- Hover scale effects
- Subtle fade-ins
- Slide animations

---

## Implementation Phases

### Phase 1: Foundation ✅ IN PROGRESS

**Tasks**:
- [x] Create new branch `feature/ui-refactoring-tailwind-themes`
- [x] Create tracking document (this file)
- [x] Analyze 3bstudio.fr for design inspiration
- [ ] Install Tailwind CSS in Blazor project
- [ ] Configure `tailwind.config.js` with custom themes
- [ ] Setup PostCSS and build pipeline
- [ ] Create `PartyTheme` enum in DataEnum project
- [ ] Add `Theme` property to `PartyDto`

**Status**: 30% complete

---

### Phase 2: Theme System Architecture

**Tasks**:
- [ ] Create `IThemeStyleService` interface
- [ ] Implement `ThemeStyleService` for CSS generation
- [ ] Create `ThemeProvider.razor` component (injects theme-specific CSS)
- [ ] Create theme configuration files (JSON)
  - [ ] `cyberpunk2077.json`
  - [ ] `3bstudio.json`
- [ ] Update `IPartyManagementService` to handle theme selection
- [ ] Register services in DI container

**Status**: Not started

---

### Phase 3: Design System Components

**Tasks**:
- [ ] Create base Tailwind components in `Shared/DesignSystem/Components/`
  - [ ] Button variants (primary, secondary, danger, ghost)
  - [ ] Card component (with theme support)
  - [ ] Input components (text, radio, checkbox)
  - [ ] Modal/Dialog component (refactor ConfirmDialog)
  - [ ] Alert/Toast component
  - [ ] Loader/Spinner component
  - [ ] Badge component
- [ ] Create layout components
  - [ ] Container
  - [ ] Grid
  - [ ] Stack (vertical/horizontal)

**Status**: Not started

---

### Phase 4: Layout Refactoring

**Tasks**:
- [ ] Refactor `MainLayout.razor` with Tailwind
  - [ ] Remove Bootstrap classes
  - [ ] Add Tailwind utility classes
  - [ ] Integrate ThemeProvider
  - [ ] Update responsive breakpoints
- [ ] Refactor `PlayerLayout.razor` with Tailwind
  - [ ] Simplify for immersive gameplay
  - [ ] Theme-aware styling
- [ ] Refactor `NavMenu.razor` with Tailwind
  - [ ] Modern navigation design
  - [ ] Theme-specific colors
  - [ ] Mobile-responsive

**Status**: Not started

---

### Phase 5: Page Refactoring - Game Pages

**Tasks**:
- [ ] **Index.razor** (Home/Party Creation)
  - [ ] Hero section with theme support
  - [ ] Party creation wizard
  - [ ] Theme preview cards
- [ ] **PartyTeamCreation.razor**
  - [ ] Themed form design
  - [ ] Animations
- [ ] **PartyTeamPlay.razor** (Main Gameplay)
  - [ ] Immersive themed layout
  - [ ] Scoreboard redesign
  - [ ] Response components (Buzzer, Proposition, QCM) with themes
  - [ ] Theme grid redesign
- [ ] **PartyThemes.razor**
  - [ ] Theme-aware theme selection grid (meta!)
  - [ ] Visual feedback

**Status**: Not started

---

### Phase 6: Page Refactoring - Admin Pages

**Tasks**:
- [ ] **Admin.razor** (Party Management)
  - [ ] Dashboard cards instead of tables
  - [ ] Statistics widgets
  - [ ] Theme-aware design
- [ ] **PartyAdmin.razor** (Party Control)
  - [ ] **Theme selector dropdown** (key feature!)
  - [ ] Real-time theme preview
  - [ ] Modern control panel
  - [ ] Team cards redesign
- [ ] **AdminQuestions.razor**
  - [ ] Drag-and-drop file upload
  - [ ] Question cards
  - [ ] CRUD interface

**Status**: Not started

---

### Phase 7: Component Refactoring

**Tasks**:
- [ ] **Buzzer.razor**
  - [ ] Cyberpunk: Neon glow, glitch effects
  - [ ] 3bstudio: Clean, professional button
  - [ ] Animation improvements
- [ ] **Proposition.razor**
  - [ ] Themed input styling
  - [ ] Focus states with theme colors
- [ ] **QCM.razor**
  - [ ] Themed card design
  - [ ] Radio button styling
- [ ] **ConfirmDialog.razor**
  - [ ] Tailwind modal with theme support
  - [ ] Backdrop blur effects

**Status**: Not started

---

### Phase 8: Theme-Specific Features

**Tasks**:
- [ ] **Cyberpunk 2077 Theme**
  - [ ] Scanline overlay component
  - [ ] Glitch text effect component
  - [ ] Neon glow utility classes
  - [ ] Grid background pattern
  - [ ] Holographic gradient borders
- [ ] **3bstudio Theme**
  - [ ] Wave decoration SVG component
  - [ ] Professional icon set integration
  - [ ] Gradient headers
  - [ ] Yellow accent highlights

**Status**: Not started

---

### Phase 9: Testing & Polish

**Tasks**:
- [ ] Test theme switching in real-time
- [ ] Test multi-party with different themes
- [ ] Responsive testing (mobile, tablet, desktop)
  - [ ] Cyberpunk theme responsiveness
  - [ ] 3bstudio theme responsiveness
- [ ] Accessibility audit (contrast ratios for both themes)
- [ ] Performance testing (CSS bundle size)
- [ ] Cross-browser testing
- [ ] Fix any visual bugs

**Status**: Not started

---

### Phase 10: Documentation & Cleanup

**Tasks**:
- [ ] Update CLAUDE.md with new architecture
- [ ] Create THEME_SYSTEM.md documentation
- [ ] Document component usage examples
- [ ] Remove old Bootstrap files
- [ ] Clean up unused CSS
- [ ] Update XML documentation
- [ ] Build final version

**Status**: Not started

---

## Technical Decisions

### Tailwind CSS Configuration

```javascript
// tailwind.config.js
module.exports = {
  content: [
    './**/*.razor',
    './**/*.html',
    './**/*.cshtml'
  ],
  theme: {
    extend: {
      colors: {
        // Cyberpunk theme
        'cp-cyan': '#00F0FF',
        'cp-pink': '#FF00FF',
        'cp-yellow': '#FFFF00',
        'cp-bg-dark': '#0A0A0A',
        'cp-bg-mid': '#1A1A1A',

        // 3bstudio theme
        '3b-blue': '#1E3A8A',
        '3b-yellow': '#FBBF24',
        '3b-gray': '#F3F4F6',
      },
      fontFamily: {
        'cyberpunk': ['Orbitron', 'Rajdhani', 'sans-serif'],
        'mono-cyber': ['Roboto Mono', 'monospace'],
        '3b-sans': ['Inter', 'Poppins', 'sans-serif'],
      },
      boxShadow: {
        'neon-cyan': '0 0 10px #00F0FF, 0 0 20px #00F0FF',
        'neon-pink': '0 0 10px #FF00FF, 0 0 20px #FF00FF',
      },
    },
  },
  plugins: [],
}
```

### Theme Service Interface

```csharp
public interface IThemeStyleService
{
    /// <summary>
    /// Gets the CSS class prefix for a specific theme.
    /// </summary>
    string GetThemePrefix(PartyTheme theme);

    /// <summary>
    /// Generates theme-specific CSS variables.
    /// </summary>
    string GenerateThemeCss(PartyTheme theme);

    /// <summary>
    /// Gets the theme configuration from JSON.
    /// </summary>
    Task<ThemeConfig> GetThemeConfigAsync(PartyTheme theme, CancellationToken cancellationToken);
}
```

### ThemeProvider Component

```razor
@* ThemeProvider.razor *@
@inject IThemeStyleService ThemeStyleService

@if (!string.IsNullOrEmpty(themeCss))
{
    <style>
        @((MarkupString)themeCss)
    </style>
}

<div class="theme-@themePrefix">
    @ChildContent
</div>

@code {
    [Parameter] public PartyTheme Theme { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private string themeCss = string.Empty;
    private string themePrefix = string.Empty;

    protected override void OnParametersSet()
    {
        themePrefix = ThemeStyleService.GetThemePrefix(Theme);
        themeCss = ThemeStyleService.GenerateThemeCss(Theme);
    }
}
```

---

## CSS Organization

```
wwwroot/css/
├── tailwind.css          # Input file with @tailwind directives
├── app.css              # Generated output (minified)
├── themes/
│   ├── base.css         # Shared base styles
│   ├── cyberpunk.css    # Cyberpunk-specific overrides
│   └── 3bstudio.css     # 3bstudio-specific overrides
└── animations/
    ├── glitch.css       # Glitch effect keyframes
    ├── neon-pulse.css   # Neon pulse animation
    └── scanlines.css    # Scanline overlay
```

---

## Migration Strategy

### Bootstrap → Tailwind Class Mapping

| Bootstrap | Tailwind |
|-----------|----------|
| `container` | `container mx-auto px-4` |
| `row` | `flex flex-wrap` |
| `col-md-6` | `w-full md:w-1/2` |
| `btn btn-primary` | `px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700` |
| `card` | `bg-white rounded-lg shadow-md p-6` |
| `alert alert-danger` | `bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded` |
| `table table-striped` | `w-full border-collapse` (custom table component) |
| `modal` | `fixed inset-0 z-50 flex items-center justify-center` |
| `navbar` | `flex items-center justify-between p-4` |

---

## Known Challenges

1. **Blazor + Tailwind JIT**: Need to configure PurgeCSS correctly to avoid missing classes
2. **Dynamic theme switching**: Must inject CSS dynamically without page reload
3. **Scoped CSS migration**: `.razor.css` files need to be converted to Tailwind utilities
4. **Font loading**: Custom fonts (Orbitron, Rajdhani) need CDN or local hosting
5. **Animation performance**: Neon glow effects may impact performance on low-end devices

---

## Progress Tracking

### Overall Completion: 5%

- **Phase 1**: 30% ✅ IN PROGRESS
- **Phase 2**: 0%
- **Phase 3**: 0%
- **Phase 4**: 0%
- **Phase 5**: 0%
- **Phase 6**: 0%
- **Phase 7**: 0%
- **Phase 8**: 0%
- **Phase 9**: 0%
- **Phase 10**: 0%

---

## Next Steps

1. Install Tailwind CSS via npm
2. Configure tailwind.config.js
3. Setup build pipeline (PostCSS)
4. Create PartyTheme enum
5. Start building base components

---

## Notes & Decisions Log

**2026-01-03**:
- Branch created: `feature/ui-refactoring-tailwind-themes`
- Analyzed 3bstudio.fr: Black/Blue/Yellow palette, modern clean design
- Defined Cyberpunk 2077 theme: Neon cyan/pink/yellow, dark backgrounds
- Theme system will use dynamic CSS injection via ThemeProvider component
- Each party will store its theme in PartyDto.Theme property

---

## Resources

### Fonts (CDN)
- **Orbitron**: https://fonts.google.com/specimen/Orbitron
- **Rajdhani**: https://fonts.google.com/specimen/Rajdhani
- **Roboto Mono**: https://fonts.google.com/specimen/Roboto+Mono
- **Inter**: https://fonts.google.com/specimen/Inter
- **Poppins**: https://fonts.google.com/specimen/Poppins

### Tailwind Plugins
- `@tailwindcss/forms`: Better form styling
- `@tailwindcss/typography`: Rich text styling
- `tailwind-scrollbar`: Custom scrollbar styling

### Design References
- Cyberpunk 2077 UI: https://www.behance.net/gallery/92723219/Cyberpunk-2077-UI-Concept
- 3bstudio.fr: https://3bstudio.fr

---

**Last Updated**: 2026-01-03
**Status**: Active Development
