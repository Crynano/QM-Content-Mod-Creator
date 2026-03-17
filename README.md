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
- Reduces repetitive setup so you can iterate faster.
- Works through simple in-game console commands.

## Quick Commands
| Command | What it does |
| --- | --- |
| `create-mod "PathToAFolder"` | Creates a new mod template folder |
| `import-mod "PathToAFolder"` | Imports your mod into the game |
| `export-weapons "PathToAFolder"` | Exports 250+ in-game weapons for reference |

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

6. If you see `Imported Mod Successfully!`, your mod loaded correctly.
7. If import fails, check `Player.log` and look for exceptions:

```text
C:\Users\<yourUser>\AppData\LocalLow\Magnum Scriptum Ltd\Quasimorph\Player.log
```

## Upload to Steam Workshop
1. Create your mod using NBK_RedSpy's template.
2. In code, create a hook for `AfterConfigLoaded` and call:

```csharp
QM_ImporterAPI.Services.ImporterApi.LoadModFromContext(IModContext);
```

3. Build your mod and locate the output `.dll`.
4. Move your mod `Assets` folder (JSON, images, audio, etc.) into the same folder as the `.dll`.
5. Publish to Workshop and subscribe to verify it loads correctly.

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

## Troubleshooting
- Weapon does not load even when command reports success:
  Check `Player.log` for hidden exceptions and verify JSON field names. Every import-mod process leaves a message with errors and failed steps.
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
