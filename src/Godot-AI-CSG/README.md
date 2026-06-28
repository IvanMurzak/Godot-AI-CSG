# CSG Tools

AI MCP tools for Godot **CSG** (Constructive Solid Geometry), for [Godot-MCP / AI Game Developer](https://github.com/IvanMurzak/Godot-MCP).

A **source-only** MCP tool extension: the package ships C# source (no compiled DLL, no bundled Godot)
that compiles inside your Godot project against your own GodotSharp, so it never locks you to a Godot
version. Verified to compile into a consumer project across Godot 4.3 … 4.5.

## Install

Requires the core [`godot_mcp`](https://github.com/IvanMurzak/Godot-MCP) addon in your Godot C# project.

```bash
# via the godot-cli (resolves from the shared catalog, edits your .csproj, rebuilds)
godot-cli install-extension com.IvanMurzak.Godot.MCP.CSG

# …or add the reference manually and rebuild:
#   <PackageReference Include="com.IvanMurzak.Godot.MCP.CSG" Version="0.1.0" />
```

…or pick it from the **Extensions** dock inside the Godot editor.

After a rebuild, the extension's `[AiToolType]` tool family is auto-discovered — no registry edit.

## Tools

| Tool | Kind | Description |
| --- | --- | --- |
| `csg-defaults` | pure-managed | Return the recommended starter config (size / radius / height / segments) for a CSG `box`, `sphere`, `cylinder`, or `combiner`. |
| `csg-box-create` | editor | Create a `CsgBox3D` in the edited scene; seed `sizeX`/`sizeY`/`sizeZ` (clamped to > 0). |
| `csg-sphere-create` | editor | Create a `CsgSphere3D`; seed `radius` (> 0) and `radialSegments` (>= 3). |
| `csg-cylinder-create` | editor | Create a `CsgCylinder3D`; seed `radius` and `height` (both > 0). |
| `csg-combiner-create` | editor | Create a `CsgCombiner3D` container that groups child shapes for a boolean op. |
| `csg-set-operation` | editor | Set a CSG node's boolean operation (`Union` / `Intersection` / `Subtraction`). |
| `csg-get` | editor | Read a CSG node's scalar config (read-only). |

All editor tools marshal every Godot call onto the editor main thread; values are clamped to valid
ranges before they touch a node.

License: Apache-2.0.
