# ReEdited

**Build your own corner of Hell.**

ReEdited is a community continuation of [UltraEditor](https://github.com/2duviz2/UltraEditor), an in-game level editor and custom level loader for **ULTRAKILL**.

The goal is to make creating levels more flexible and approachable: better building tools, more objects and events, scripting, and support for imported models. Longer term, ReEdited aims to bring lightweight modeling and animation tools directly into the editor.

> **Early development:** the current work establishes a buildable foundation for the continuation. A Release build has been verified against a local Steam installation, but in-game testing of this branch is still pending. The roadmap below describes planned features, not features already available.

## The starting point

UltraEditor provides the foundation: an in-game editor, object placement and inspection, game prefab browsing, custom level saving/loading, and gameplay components such as triggers, moving platforms, lights, and music.

ReEdited currently adds a configurable local build setup, C# 13 compatibility for the .NET 9 SDK, and a categorized asset browser. The displayed plugin name is **ReEdited**, version **0.1.0**. The DLL name, plugin GUID, namespaces, and level storage paths retain their original UltraEditor identifiers for compatibility.

The asset browser opens in `Assets/ReEdited/`, with alphabetically sorted folders for enemies, decorations, interactive objects, doors, obstacles, sandbox objects, effects, and special rooms. Empty categories are hidden. `Recommended` keeps the original editor's curated selection. Use the existing parent-folder button to return to `Assets/` and browse the original asset folders.

The first tile, **Search all assets...**, searches names throughout the catalog, ignoring case. Results keep their original spawn keys and appear alphabetically without duplicates. Up to 100 results are displayed; narrow the query if more match. Clear the text or use the parent-folder button to return to your previous folder. Camera movement and editing shortcuts are blocked while typing in the search field.

**Right-click an asset** to add or remove it from your favorites; left-click still places it. Favorite assets display `[*]` before their names and appear in `Assets/ReEdited/Favorites/`. Favorites are saved locally between sessions, including when marked from search results. They are personal preferences and are not included in shared levels. Saved favorites that are unavailable in the current game catalog are hidden.

Categories collect existing game assets; they do not add new models or guarantee that every prefab works independently in a custom level. Asset keys used for placement and saving remain unchanged. The category logic is tested, but the in-game layout and placement workflow still require verification.

The inherited editor is unfinished. Save compatibility and complete property persistence still need work, so keep backups of levels when testing development builds. The inherited community level browser uses the original project's online services; their availability has not been verified.

## Roadmap

These are development goals. Scope and implementation may change as the underlying systems are tested.

| Stage | Planned work |
| --- | --- |
| Building tools and content | Build on the categorized browser, search, favorites, and configurable movement snapping with improved duplication and more objects, enemies, and events. |
| Script editor | In-game editing, event templates, object references, compilation, and an error console. C# is the initial language proposal. |
| OBJ and FBX import | Import models with scale and rotation controls, materials, textures, and optional collision generation. Start with OBJ, then validate a runtime FBX importer. |
| Lightweight model editor | Create and edit primitives, vertices, edges, and faces; add extrusion, cuts, joining, materials, and undo/redo. |
| Animation editor | After modeling: a timeline, transform keyframes, curves, loops, and playback triggered by events or scripts. Bones, rigging, and character animation come later. |

Models, textures, scripts, and animations should travel with the level rather than depend on files elsewhere on the creator's computer. Saving and reopening levels reliably is part of every stage.

## Editor settings and mod dependencies

ReEdited requires [Thorn Core 0.5.0 or newer](https://thunderstore.io/c/ultrakill/p/end_4/Thorn_Core/) in addition to BepInEx 5. Install the complete package and its dependencies through a mod manager. The pinned package requires FixPluginTypesSerialization 1.0.0, NukeLib 0.12.0, Notiffy 0.1.4, and Fireman 0.1.1. The build reference DLL alone does not include Thorn's UI assets or runtime dependencies. Thorn's separate gameplay and HUD modules are not required.

Open Thorn's menu with **Right Shift** (its default shortcut), then find **ReEdited** under **Modules > Utility** or search for it. Its settings include:

- Editor toggle, selection/move/scale/rotation tools, UI toggle, delete, and cube creation keys, with optional modifier keys.
- Editor camera movement speed, fast movement multiplier, and look sensitivity multiplier.
- Grid snapping and grid size under **Building tools**.
- Undo and redo shortcuts for object edits (Left Ctrl+Z and Left Ctrl+Y by default).

Camera look sensitivity multiplies the existing game mouse sensitivity. Settings are saved by Thorn and follow its active profile. Changes apply immediately. Editor controls are blocked while Thorn's menu is open. This integration styles the settings menu; the inherited editor windows still use their existing interface.

**Grid snapping** is disabled by default, with a default grid size of **0.25 world units**. Enable it in **Building tools** to snap movement with the Move tool. Grid size ranges from 0.01 to 100 world units. Global arrows snap only the coordinate being moved, using the world origin as the grid origin; the other two coordinates stay unchanged. Local arrows move in grid-sized steps along their own direction, measured from the drag's starting position, so rotated objects stay on the chosen axis.

When grid snapping is disabled, hold **Left Ctrl** to snap temporarily with the configured grid size; **Left Ctrl + Left Shift** uses one-unit steps. Scale and rotation retain their inherited Ctrl shortcuts. Changing grid settings does not reposition existing objects until you drag them. Grid settings are personal Thorn preferences and are not stored in shared levels.

**Right-click context menu:** quickly click the right mouse button in the editor viewport to open a dark menu beside the cursor. Clicking an editable object selects it; clicking empty space keeps your selection. **Create** offers a cube, floor, or wall. The menu also offers **Duplicate**, **Delete**, **Undo**, **Redo**, **Move**, **Rotate**, **Scale**, and **Focus selected**. Actions that need a selection or history are disabled when unavailable. Escape or a click outside closes the menu. Holding the right mouse button for 0.2 seconds or dragging it still controls camera look. Right-clicking asset cards keeps its favorites behavior.

**Undo/redo** records cube/floor/wall creation, prefab placement from the asset catalog, duplication, deletion, and Move/Scale/Rotate tool drags, including snapped movement. These actions also enter history when used through the original editor buttons or shortcuts. Each completed drag is one action. Use the context menu, **Left Ctrl+Z**, or **Left Ctrl+Y**; the keyboard shortcuts can be changed in Thorn. The history holds the last 100 actions for the current editor instance and is not saved with levels. New recorded edits discard the redo branch. Undoable deletions temporarily retain the same inactive object instance, including children, under an internal history holder; these objects are excluded from level serialization and billboards. Retained objects are released when their history is discarded. Loading or clearing a level clears the history. Reparented objects and transforms changed outside the recorded tool workflow are skipped when their saved state no longer matches. Inspector edits, grouping, and hierarchy changes are not recorded yet. Keyboard undo/redo is blocked during a drag, while the editor is closed, while a modal blocker, context menu, or Thorn's menu is open, and when an input field has focus.

[UnityExplorer](https://github.com/sinai-dev/UnityExplorer) is an optional development tool for inspecting and debugging game objects. ReEdited does not require it.

The settings integration compiles against Thorn Core 0.5.0. In-game panel rendering, key reassignment, profile switching, and persistence still require manual verification.

## Build from source

### Requirements

- The **.NET 9 SDK** for the current C# 13 source.
- An installed copy of **ULTRAKILL**, including `ULTRAKILL_Data/Managed`.
- Access to the NuGet feeds configured in the project to restore its existing dependencies.
- **BepInEx 5** installed in the game to run the resulting mod. The project references BepInEx 5.4.21.
- **Thorn Core 0.5.0 or newer** and its dependencies installed in the game to run the mod, with `ThornClient.dll` available as a build reference.

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
   ```

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

Grid rounding checks also run without the game:

```powershell
dotnet run --project tests/GridSnap.Tests/GridSnap.Tests.csproj -c Release
```

Undo/redo ordering, invalid actions, history limits, and branching checks:

```powershell
dotnet run --project tests/EditHistory.Tests/EditHistory.Tests.csproj -c Release
```

For an in-game history check, create an object, move/scale/rotate it, duplicate it, then delete the duplicate. Undo the sequence and redo it, using both the context menu and keyboard. Repeat with grid snapping, child objects, and a prefab from the catalog. Undo an edit, make a new edit, and verify that redo is unavailable. Select another object before undoing to verify the original target changes. Save while an object is deleted or its creation is undone, then reload to verify that retained history objects are absent from the level. Verify inactive prefabs stay inactive after restoration, parent relationships survive, and clearing/loading a level removes history. Test input fields and Thorn's menu to verify Ctrl+Z/Y do not edit objects while typing. Check menu placement at screen edges, disabled actions without selection/history, Escape/outside-click dismissal, short right clicks, held right-button camera look, and asset-card favorites. These Unity interaction checks still require manual verification.

For an in-game snapping check, enable Grid snapping in Thorn, select the Move tool, and drag each global axis on an object whose position is off the grid. Verify that only the moved coordinate aligns, including negative coordinates. Change the grid size and test temporary Ctrl snapping with the setting disabled, plus Ctrl+Shift one-unit steps. Rotate an object and use local arrows to verify stepped movement stays along the selected arrow. Disable snapping to check free movement, then save and reopen the level to verify positions. Restart the game and switch Thorn profiles to check the snapping preferences. These interaction checks still require manual verification.

For an in-game check, open the editor's asset browser, navigate through categories and back to the original folders, place a recommended object and a categorized prefab, then save and reopen a copy of the level. Check that navigation leaves no stale buttons and that objects retain their original asset references. Mark an asset from a category and from search results, verify its `[*]` label and Favorites entry, then restart the game to check persistence. Remove a favorite from the Favorites folder and verify that left-click placement still works and right-clicking a card does not rotate the camera. In-game behavior and PlayerPrefs persistence still require this manual verification.

## Contributing

For settings changes, open Thorn's ReEdited configuration panel, reassign a tool key with and without a modifier, adjust each camera value, switch profiles, and restart the game. Verify that the values are restored and that typing or recording keys in the panel does not delete objects, move the camera, or toggle the editor. Set a shortcut to None to check that it is disabled. Check that controls resume when Thorn's menu closes.

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
