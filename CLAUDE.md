# CLAUDE.md — Godot-AI-CSG

A **Godot-MCP extension**: an MCP tool family for Godot's built-in **CSG** (Constructive Solid Geometry)
3D nodes — the `CSGShape3D` family (`CSGBox3D`, `CSGSphere3D`, `CSGCylinder3D`, `CSGCombiner3D`) — shipped
as a **source-only NuGet package** (`com.IvanMurzak.Godot.MCP.CSG`) that compiles inside a consumer's
Godot project against the consumer's own GodotSharp. Created from
[`Godot-AI-Tools-Template`](https://github.com/IvanMurzak/Godot-AI-Tools-Template). The packaging recipe
is the load-bearing detail — read `docs/source-only-nuget-recipe.md`.

## Layout

- `src/Godot-AI-CSG/` — the source-only package (`Godot.NET.Sdk`).
  - `Runtime/Tools/Tool_CSG.cs` — the `[AiToolType]` family (one partial class).
  - `Runtime/Tools/Tool_CSG.Ids.cs` — all tool-id consts (pure-managed; pinned by tests).
  - `Runtime/CSG/` — pure-managed support types: CSG operation parsing/mapping and value rules
    (all unit-tested, no Godot native API).
  - `Editor/Tools/Tool_CSG.{Editor,BoxCreate,SphereCreate,CylinderCreate,CombinerCreate,SetOperation,Get}.cs`
    — editor tools behind `#if TOOLS` (touch `EditorInterface`/live nodes; main-thread-marshalled; E2E-verified).
  - `build/com.IvanMurzak.Godot.MCP.CSG.props` — the source-injection props (auto-imported by NuGet in
    the consumer; MUST stay named `<PackageId>.props`).
- `tests/Godot-AI-CSG.Tests/` — xUnit specs for the pure-managed sources only (no Godot binary).
- `testbed/CSG-Testbed.csproj` — a consumer `Godot.NET.Sdk` project that restores the local-packed
  package; `dotnet build` of it is the source-injection proof.

## Tools

| Tool | Kind | File |
| --- | --- | --- |
| `csg-box-create` | editor | `Editor/Tools/Tool_CSG.BoxCreate.cs` |
| `csg-sphere-create` | editor | `Editor/Tools/Tool_CSG.SphereCreate.cs` |
| `csg-cylinder-create` | editor | `Editor/Tools/Tool_CSG.CylinderCreate.cs` |
| `csg-combiner-create` | editor | `Editor/Tools/Tool_CSG.CombinerCreate.cs` |
| `csg-set-operation` | editor | `Editor/Tools/Tool_CSG.SetOperation.cs` |
| `csg-get` | editor | `Editor/Tools/Tool_CSG.Get.cs` |

Namespace note: `CSG` is the feature name, not a single engine type — the engine nodes are named
`CSGBox3D` / `CSGSphere3D` / `CSGCombiner3D` etc., so the root namespace `com.IvanMurzak.Godot.MCP.CSG`
does NOT shadow any Godot type (no per-file `using GdCSG = Godot.CSG;` alias is needed, unlike `Animation`
or `GridMap`).

## Build / test (no Godot binary)

```bash
dotnet build src/Godot-AI-CSG/Godot-AI-CSG.csproj   # source-only package compiles tools
dotnet test  tests/Godot-AI-CSG.Tests/Godot-AI-CSG.Tests.csproj
dotnet pack  src/Godot-AI-CSG/Godot-AI-CSG.csproj -p:Version=0.0.0-ci -o local-nuget
dotnet build testbed/CSG-Testbed.csproj             # consumes the local package (injection proof)
```

`Godot.NET.Sdk` supplies GodotSharp from NuGet, so no Godot install is needed to build/test/pack or to
prove the source-injection recipe (the testbed build is a faithful proxy for `godot --build-solutions`).
The recipe is verified to compile into the consumer across the CI Godot-version matrix (4.3 / 4.4 / 4.5).
When proving locally, note `dotnet pack` re-uses the **global NuGet cache** for an already-cached version:
if you re-pack the same `Version`, clear `~/.nuget/packages/com.ivanmurzak.godot.mcp.csg/<ver>` (or pack a
unique version) before re-restoring the testbed, or you'll silently build the stale cached source.

## Conventions

- Root namespace `com.IvanMurzak.Godot.MCP.CSG`. Every `.cs` starts with the Apache-2.0 header.
- Pure-managed cores (no Godot native API) → `Runtime/` (outside `#if TOOLS`, unit-testable); editor-driving
  tools → `Editor/` (behind `#if TOOLS`, every Godot call via `MainThread.Instance.Run(...)`, E2E-verified).
- The package declares ONLY the `com.IvanMurzak.McpPlugin` / `com.IvanMurzak.ReflectorNet` min-version
  deps; **GodotSharp must never become a package dependency** (CI asserts the nuspec). Keep the MCP pins in
  lockstep with the core Godot-MCP addon; bump with `commands/update-core.ps1`.
- One `[AiToolType] partial class Tool_CSG`; one `[AiTool]` method per partial-class file. New
  pure-managed sources must be added to the test csproj `<Compile Include>` list to be unit-tested.

## Find detail in

- `docs/source-only-nuget-recipe.md` — the packaging recipe (the centerpiece) + the consumer story.
- `docs/ci.md` — workflows, the version gate, multi-Godot matrix, required publishing config.
