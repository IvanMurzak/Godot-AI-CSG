<h1 align="center">Godot AI CSG</h1>

<p align="center">
  AI <b>MCP tools</b> for Godot <b>Constructive Solid Geometry</b> (the CSGShape3D family) — an extension for
  <a href="https://github.com/IvanMurzak/Godot-MCP">Godot-MCP / AI Game Developer</a>.
</p>

`Godot-AI-CSG` adds a focused MCP tool family for Godot's built-in **CSG** (Constructive Solid Geometry)
3D nodes — `CSGBox3D`, `CSGSphere3D`, `CSGCylinder3D`, and the `CSGCombiner3D` that boolean-combines its
children (Union / Intersection / Subtraction). The tools are authored in C# with `[AiToolType]` /
`[AiTool]` (the same model as Unity-MCP and the core Godot-MCP addon), and shipped as a **source-only
NuGet package** that compiles inside any consumer's Godot project against the consumer's own GodotSharp
— no bundled Godot, no version lock. Created from
[`Godot-AI-Tools-Template`](https://github.com/IvanMurzak/Godot-AI-Tools-Template).

## Tools

| Tool | Kind | Description |
| --- | --- | --- |
| `csg-box-create` | editor (`#if TOOLS`) | Create a `CSGBox3D` in the edited scene; `size` (x, y, z); optional parent, name. |
| `csg-sphere-create` | editor (`#if TOOLS`) | Create a `CSGSphere3D`; `radius`, `radial_segments`; optional parent, name. |
| `csg-cylinder-create` | editor (`#if TOOLS`) | Create a `CSGCylinder3D`; `radius`, `height`; optional parent, name. |
| `csg-combiner-create` | editor (`#if TOOLS`) | Create a `CSGCombiner3D` — the root container that boolean-combines its CSG children. |
| `csg-set-operation` | editor (`#if TOOLS`) | Set a CSG node's boolean operation (`Union` / `Intersection` / `Subtraction`). |
| `csg-get` | editor (`#if TOOLS`) | Read a CSG node's scalar config (read-only). |

Pure-managed support (operation parsing, the tool-id consts) lives under `src/Godot-AI-CSG/Runtime/` and
is CI-unit-tested; editor-driving tools live under `Editor/` behind `#if TOOLS` and marshal every Godot
call onto the editor main thread via `MainThread.Instance.Run(...)`.

## Install (in a consumer Godot project)

Requires the core [`godot_mcp`](https://github.com/IvanMurzak/Godot-MCP) addon. Then either:

- **Extensions dock** — pick it inside the Godot editor (Install → adds the `<PackageReference>` → rebuild).
- **CLI** — `godot-cli install-extension com.IvanMurzak.Godot.MCP.CSG`.
- **By hand** — add `<PackageReference Include="com.IvanMurzak.Godot.MCP.CSG" Version="x.y.z" />`
  to the consumer `.csproj` and rebuild.

After a rebuild the `[AiToolType]` tool family is auto-discovered — no registry edit.

## Build & test (no Godot binary needed)

`Godot.NET.Sdk` pulls GodotSharp from NuGet, so the package builds and unit-tests headless:

```bash
dotnet build src/Godot-AI-CSG/Godot-AI-CSG.csproj            # compiles tools (Godot API resolves)
dotnet test  tests/Godot-AI-CSG.Tests/Godot-AI-CSG.Tests.csproj   # pure-managed unit tests
dotnet pack  src/Godot-AI-CSG/Godot-AI-CSG.csproj -p:Version=0.0.0-ci -o local-nuget
dotnet build testbed/CSG-Testbed.csproj                      # consumer build = source-injection proof
```

The testbed build proves the source-injection recipe: the package's `.cs` are injected as `<Compile>`
items into the consumer and compile against the consumer's own GodotSharp. CI runs this across a
multi-Godot-version matrix; an end-to-end leg additionally boots real headless Godot, installs the core
addon, and (once a local MCP server is wired) calls each tool via `godot-cli run-tool`.

## Docs

- `docs/source-only-nuget-recipe.md` — the packaging recipe (the centerpiece).
- `docs/ci.md` — workflows, the version gate, the multi-Godot matrix, required secrets.
- `CLAUDE.md` — maintainer notes.

## Publish

Source-only, version-gated release (see `docs/ci.md`): publishing uses NuGet **Trusted Publishing**
(OIDC, no stored API key) — bump `<Version>` (`commands/bump-version.ps1 -NewVersion x.y.z`), merge to
`main`; `release.yml` runs the full matrix, publishes the package to NuGet, and cuts an atomic GitHub
Release.

License: **Apache-2.0**.
