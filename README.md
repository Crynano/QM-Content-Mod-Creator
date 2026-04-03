# Content Mod Creator - New Weapon and Item Importer API

[![Support on Ko-fi](https://img.shields.io/badge/Support-Ko--fi-ff5f5f?logo=kofi&logoColor=white)](https://ko-fi.com/crynano)
[![Author: Crynano](https://img.shields.io/badge/Author-Crynano-2f6f91)](https://github.com/Crynano)

Content Mod Creator helps you create Quasimorph weapon and item mods without having to touch code!
It's a straight upgrade from the Weapon and Item Importer API I've previously developed.
It handles folder creation, settings, image, audio and configuration so you can focus on building fun content! (Can't wait to see more content mods)

## Table of Contents
- [Features](#features)
- [Quick Commands](#quick-commands)
- [Create a Mod](#create-a-mod-from-scratch)
- [Import a Mod](#import-a-mod)
- [Upload to Steam Workshop](#upload-to-steam-workshop)
- [Quick Guide](#quick-guide)
- [Tips and Tricks](#tips-and-tricks)
- [Restrictions](#restrictions)
- [Troubleshooting](#troubleshooting)
- [Support](#support)
- [Special Thanks](#special-thanks)
- [Other Mods](#my-other-quasimorph-mods)

## Features
- Creates mod folder structure and settings for you.
- Imports JSON, images, and audio assets.
- Supports weapons, armor, ammo, firemodes, explosions, and consumables.
- Reduces repetitive setup so you can iterate faster.
- Works through simple in-game console commands.

## Quick Commands
| Command | What it does |
| --- | --- |
| `create-mod "PathToAFolder"` | Creates a full mod template folder (all content types) |
| `create-weapon-mod "PathToAFolder"` | Creates a weapon-focused mod template folder |
| `create-consumable-mod "PathToAFolder"` | Creates a consumable-focused mod template folder |
| `import-mod "PathToAFolder"` | Imports your mod into the game |
| `export-weapons "PathToAFolder"` | Exports 250+ in-game weapons for reference |
| `export-armor "PathToAFolder"` | Exports in-game armor records for reference |

## Create a Mod from Scratch
1. Install Content Mod Creator
2. Install a developer console mod.
3. Start Quasimorph.
4. In the main menu, open developer console with `~` (key left of `1`).
5. Run:

```console
create-mod "C:/Temp/Mod"
```
6. Navigate to that folder and edit the generated files.

## Import a Mod
1. Install Content Mod Creator
2. Install a developer console mod.
3. Start Quasimorph.
4. In the main menu, open developer console with `~`.
5. Run:

```console
import-mod "C:/Temp/Mod"
```

6. After import, the console shows a result summary with execution time, a list of loaded content, any warnings, and errors.
7. If import fails, errors appear in red directly in the developer console. For the full trace, check `Player.log`:

```text
C:\Users\<yourUser>\AppData\LocalLow\Magnum Scriptum Ltd\Quasimorph\Player.log
```

## Upload to Steam Workshop
1. Create your mod using NBK_RedSpy's template.
2. Add a project reference to `QM_ImporterAPI.dll`, located at:

```text
SteamLibrary\steamapps\workshop\content\2059170\3671320495\QM_ImporterAPI.dll
```

3. In code, create a hook for `AfterConfigLoaded` and call:

```csharp
QM_ImporterAPI.Services.ImporterApi.LoadModFromContext(context);
```

> Pass `context` (the `IModContext` instance your hook receives), not the interface type itself.

4. Build your mod and locate the output `.dll`.
5. Move your mod `Assets` folder (JSON, images, audio, etc.) into the same folder as the `.dll`.
6. Publish to Workshop and subscribe to verify it loads correctly.

> This is a concise workflow summary. For full walkthroughs, check the Quasimorph Discord and Steam Guides.

## Quick Guide
### Mod
- You can create as many JSONs as you need, separate localization, crafting recipes or faction rewards at your will.
- There is no required nor fixed naming, organize folders as you wish.
- All config (records and descriptors) file extensions must be .json to be taken into account.
- Weapon ID and Descriptor ID must match for the weapon to be loaded. Same rule applies to all other configs except for crafting, which is OutputItem.
### Records
- TransformationRecords are what you could get when disassembling/dismantling the weapon.
- ItemProduceReceipt or Crafting Recipes, define costs and time to craft and upgrade the item in the spaceship. 
- FactionRewards define which faction and at what level the item will be given as reward.
### Restrictions
- You can't add custom recipes without adding a weapon first. It will be added in the future.
- Custom models can't be added unless bundled in a Unity Assetbundle file. You can find tutorials online explaining this process. Requires Unity Engine installed but no prior knowledge of it.

## Tips and Tricks
- If a variable ends with `Id`, it can copy properties from an in-game item (icons, sprites, audio, models, and more).
- Try `common_knife_1` to load the base knife values quickly.
- Always review logs, even if the console command does not show an error.
- For live logs, use the Unity External Log mod by NBK_RedSpy.
- To inspect all base game weapons:

```console
export-weapons "C:/Temp/WeaponDump"
```

### Crafting Recipes
- Crafting recipes can be finicky with ongoing saves. If a newly added recipe does not appear in-game, start a new game to verify it works.
- In a crafting recipe, the `Id` field is unused. Use `OutputItem` to specify what the recipe produces.
- `ModifyItemsGrades` defines the **total** number of each chip needed to reach the maximum upgrade level. For example:

```json
"ModifyItemsGrades": {
  "itemChip": 7,
  "mediumItemChip": 15
},
"ModifyLevelLimit": 15
```

  This means 1 Medium Item Chip per upgrade level, and 1 Item Chip every 2 upgrade levels.

### Item Chips
- Item chip lists are **merged**, not replaced. You only need to list the items you want to add — existing chip contents are preserved automatically.
- You can find base game chip definitions in `config_items` in the game data files, useful as a reference when building chip modifications.

### Creating New Chips
- To create a new chip, define a chip with a brand new unique ID. It can be added to faction rewards via the mod.
- New chips won't have a custom sprite unless you provide one. A simple starting approach is to add them as faction rewards.

### Mod Organization
- Consider splitting content into separate mods per faction or theme. There is no built-in way to toggle individual elements within a single mod, so smaller focused mods give users more control over what they load.

## Troubleshooting
- Item does not load even when command reports success:
  Check the developer console and `Player.log` for hidden exceptions and verify JSON field names. Every import-mod process prints a result summary with errors and failed steps.
- Errors during import appear in red directly in the developer console, not just in `Player.log`. If you see a red message after running `import-mod`, that is your first signal something went wrong.
- Missing images or audio:
  Verify file names, paths, and that files exist under your mod `Assets` folder.
  ## Support
If this project helps your workflow and you want to support updates:

- [Support Content Mod Creator on Ko-fi](https://ko-fi.com/crynano)

## Special Thanks
- Raigir (incredible designer)
- Lychantiure (awesome artist)
- NBK_RedSpy (god-tier modder and template creator)

Mod created by [Crynano](https://github.com/Crynano).
Feel free to ask in the Quasimorph modding community if you have questions.

## My Other Quasimorph Mods
- Mod Configuration Menu
- Expanded Faction Arsenal (EFA)
- Display Movement Speed UI
- Original Item and Weapon Importer
- Cyberpunk 2077 Rebel
