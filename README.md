# Content Mod Creator - New Weapon and Item Importer API

[![Support on Ko-fi](https://img.shields.io/badge/Support-Ko--fi-ff5f5f?logo=kofi&logoColor=white)](https://ko-fi.com/crynano)
[![Author: Crynano](https://img.shields.io/badge/Author-Crynano-2f6f91)](https://github.com/Crynano)

Content Mod Creator helps you create Quasimorph weapon and item mods without having to touch code!
It's a straight upgrade from the Weapon and Item Importer API I've previously developed.
It handles folder creation, settings, image, audio and configuration so you can focus on building fun content! (Can't wait to see more content mods)

## Table of Contents
- [Features](#features)
- [Quick Commands](#quick-commands)
- [Create a Mod from Scratch](#create-a-mod-from-scratch)
- [Import a Mod](#import-a-mod)
- [Upload to Steam Workshop](#upload-to-steam-workshop)
- [Tips and Tricks](#tips-and-tricks)
- [Troubleshooting](#troubleshooting)
- [Support](#support)
- [Special Thanks](#special-thanks)
- [Other Mods](#other-mods)

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
1. Install Importer v2.
2. Install a developer console mod.
3. Start Quasimorph.
4. In the main menu, open developer console with `~` (key left of `1`).
5. Run:

```console
create-mod "C:/Temp/Mod"
```

6. Navigate to that folder and edit the generated files.

## Import a Mod
1. Install Importer v2.
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
  Check `Player.log` for hidden exceptions and verify JSON field names.
- Missing images or audio:
  Verify file names, paths, and that files exist under your mod `Assets` folder.

## Support
If this project helps your workflow and you want to support updates:

- [Support Importer v2 on Ko-fi](https://ko-fi.com/crynano)

## Special Thanks
- Raigir (incredible designer)
- Lychantiure (awesome artist)
- NBK_RedSpy (incredible modder and template creator)

Mod created by [Crynano](https://github.com/Crynano).
Feel free to ask in the Quasimorph modding community if you have questions.

## Other Mods
- MCM
- Expanded Faction Arsenal (EFA)
