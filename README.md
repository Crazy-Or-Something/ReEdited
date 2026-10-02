# ReEdited

**Build your own corner of Hell.**

ReEdited is a community continuation of [UltraEditor](https://github.com/2duviz2/UltraEditor), an in-game level editor and custom level loader for **ULTRAKILL**.

The goal is to make creating levels more flexible and approachable: better building tools, more objects and events, scripting, and support for imported models. Longer term, ReEdited aims to bring lightweight modeling and animation tools directly into the editor.

> **Early development:** the current work establishes a buildable foundation for the continuation. A Release build has been verified against a local Steam installation, but in-game testing of this branch is still pending. The roadmap below describes planned features, not features already available.

## The starting point

UltraEditor provides the foundation: an in-game editor, object placement and inspection, game prefab browsing, custom level saving/loading, and gameplay components such as triggers, moving platforms, lights, and music.

ReEdited currently adds a configurable local build setup, C# 13 compatibility for the .NET 9 SDK, and a categorized asset browser. The compiled DLL and plugin identity still use the original UltraEditor names.

The asset browser opens in `Assets/ReEdited/`, with alphabetically sorted folders for enemies, decorations, interactive objects, doors, obstacles, sandbox objects, effects, and special rooms. Empty categories are hidden. `Recommended` keeps the original editor's curated selection. Use the existing parent-folder button to return to `Assets/` and browse the original asset folders.

The first tile, **Search all assets...**, searches names throughout the catalog, ignoring case. Results keep their original spawn keys and appear alphabetically without duplicates. Up to 100 results are displayed; narrow the query if more match. Clear the text or use the parent-folder button to return to your previous folder. Camera movement and editing shortcuts are blocked while typing in the search field.

**Right-click an asset** to add or remove it from your favorites; left-click still places it. Favorite assets display `[*]` before their names and appear in `Assets/ReEdited/Favorites/`. Favorites are saved locally between sessions, including when marked from search results. They are personal preferences and are not included in shared levels. Saved favorites that are unavailable in the current game catalog are hidden.

Categories collect existing game assets; they do not add new models or guarantee that every prefab works independently in a custom level. Asset keys used for placement and saving remain unchanged. The category logic is tested, but the in-game layout and placement workflow still require verification.

The inherited editor is unfinished. Save compatibility and complete property persistence still need work, so keep backups of levels when testing development builds. The inherited community level browser uses the original project's online services; their availability has not been verified.

## Roadmap

These are development goals. Scope and implementation may change as the underlying systems are tested.

| Stage | Planned work |
| --- | --- |
| Building tools and content | Build on the categorized browser, search, and favorites with grid snapping, improved duplication, and more objects, enemies, and events. |
| Script editor | In-game editing, event templates, object references, compilation, and an error console. C# is the initial language proposal. |
| OBJ and FBX import | Import models with scale and rotation controls, materials, textures, and optional collision generation. Start with OBJ, then validate a runtime FBX importer. |
| Lightweight model editor | Create and edit primitives, vertices, edges, and faces; add extrusion, cuts, joining, materials, and undo/redo. |
| Animation editor | After modeling: a timeline, transform keyframes, curves, loops, and playback triggered by events or scripts. Bones, rigging, and character animation come later. |

Models, textures, scripts, and animations should travel with the level rather than depend on files elsewhere on the creator's computer. Saving and reopening levels reliably is part of every stage.

## Build from source

### Requirements

- The **.NET 9 SDK** for the current C# 13 source.
- An installed copy of **ULTRAKILL**, including `ULTRAKILL_Data/Managed`.
- Access to the NuGet feeds configured in the project to restore its existing dependencies.
- **BepInEx 5** installed in the game to run the resulting mod. The project references BepInEx 5.4.21.

### Setup

1. Clone this repository and enter its directory:

   ```powershell
   git clone https://github.com/Mystic-Red/ReEdited.git
   cd ReEdited
   ```

2. Create your local path configuration:

   ```powershell
   Copy-Item UltraEditor/Paths.local.props.example UltraEditor/Paths.local.props
   ```

3. Edit `UltraEditor/Paths.local.props` and set `ULTRAKILLPath` to your game's root folder, the one containing `ULTRAKILL.exe`:

   ```xml
   <Project>
     <PropertyGroup>
       <ULTRAKILLPath>C:\YourSteamLibrary\steamapps\common\ULTRAKILL</ULTRAKILLPath>
     </PropertyGroup>
   </Project>
   ```

   Git ignores this local configuration file.

4. Build:

   ```powershell
   dotnet build UltraEditor.sln -c Release


The output is `UltraEditor/bin/Release/netstandard2.1/UltraEditor.dll`. The build does not install the mod by default.

### Install a development build

With BepInEx 5 already installed, close the game and copy the generated `UltraEditor.dll` into `BepInEx/plugins/UltraEditor/` inside your ULTRAKILL installation. Alternatively, build and copy it automatically:

```powershell
dotnet build UltraEditor.sln -c Release -p:DeployToGame=true
```

Do not load this build alongside another copy of UltraEditor: both currently share the same plugin GUID. Check `BepInEx/LogOutput.log` for loading errors before testing a level.

The inherited workflow starts with **Create level** in chapter selection and **F1** to toggle the editor. This still needs to be verified in-game for the continuation.

## Development checks

The catalog, search, and favorites filtering checks run without the game or additional test packages:

```powershell
dotnet run --project tests/AssetCatalog.Tests/AssetCatalog.Tests.csproj -c Release
```

For an in-game check, open the editor's asset browser, navigate through categories and back to the original folders, place a recommended object and a categorized prefab, then save and reopen a copy of the level. Check that navigation leaves no stale buttons and that objects retain their original asset references. Mark an asset from a category and from search results, verify its `[*]` label and Favorites entry, then restart the game to check persistence. Remove a favorite from the Favorites folder and verify that left-click placement still works and right-clicking a card does not rotate the camera. In-game behavior and PlayerPrefs persistence still require this manual verification.

## Contributing

Bug reports, reproducible test cases, and focused improvements are welcome. Use the [issue tracker](https://github.com/Mystic-Red/ReEdited/issues) to discuss problems and larger changes.

For bug reports, include your game and mod versions, reproduction steps, relevant log output, and whether the problem occurs with other mods disabled. When changing serialization or editor behavior, test saving and reopening a level as well as loading a copy of an older level.

Keep documentation, code comments, interface text, and messages in **English**.

## Credits

- **Duviz / 2duviz2 and the UltraEditor contributors** — the original editor, code, and bundled assets. [Original repository](https://github.com/2duviz2/UltraEditor).
- **Mystic-Red** — the ReEdited continuation. [ReEdited repository](https://github.com/Mystic-Red/ReEdited).
- **BepInEx and the libraries used by UltraEditor** — the modding foundation and dependencies.

ReEdited is an unofficial community project and is not affiliated with or endorsed by ULTRAKILL's developers or publisher.

## License status

No license file was found in the upstream snapshot used for this continuation. Permissions for the original code and bundled assets still need clarification before distributing a ReEdited release. This README does not assign a new license to upstream work.
