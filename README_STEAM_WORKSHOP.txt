[h1]Content Mod Creator - NEW Weapon and Item Importer API[/h1]

Content Mod Creator lets you create Quasimorph weapon and item mods without dealing with repetitive setup code.
It handles folders, settings, image importing, audio importing, and configuration loading so you can focus on making content.
Now supports weapons, armor, ammo, firemodes, explosions, consumables, traits, datadisks, implants, and mercenary class mods.

[h2]Quick Commands[/h2]

[b]Create[/b]
[list]
[*]Full mod template (all content types)
[code]create-mod "PathToAFolder"[/code]
[*]Weapon-focused template
[code]create-weapon-mod "PathToAFolder"[/code]
[*]Consumable-focused template
[code]create-consumable-mod "PathToAFolder"[/code]
[*]Mercenary class template
[code]create-merc-mod "PathToAFolder"[/code]
[*]Trait template
[code]create-trait-mod "PathToAFolder"[/code]
[/list]

[b]Import and Manage[/b]
[list]
[*]Import your mod into the game
[code]import-mod "PathToAFolder"[/code]
[*]Update existing mod files with new properties
[code]update-mod "PathToAFolder"[/code]
[*]Migrate old Weapon Importer mod files to the new format
[code]migrate-old-mod "PathToAFolder"[/code]
[*]Spawn items on the floor or in ship cargo (amount and cargo index are optional)
[code]give <itemId> [amount] [cargoIndex][/code]
[*]Remove all instances of an item from the savegame
[code]removeitem <itemId>[/code]
[/list]

[b]Export (for reference)[/b]
[list]
[*]Export 250+ base game weapons
[code]export-weapons "PathToAFolder"[/code]
[*]Export base game armor records
[code]export-armor "PathToAFolder"[/code]
[*]Export all base game item chips and datadisks
[code]export-chips "PathToAFolder"[/code]
[/list]

[h2]Create a Mod from Scratch[/h2]
[list=1]
[*]Open developer console with ~.
[*]Run:
[/list]
[code]
create-mod "C:/Temp/Mod"
[/code]
[list=1]
[*]Go to that folder and edit your generated files at your liking!
[/list]

[h2]Import a Mod[/h2]
[list=1]
[*]Open developer console with ~.
[*]Run:
[/list]
[code]
import-mod "C:/Temp/Mod"
[/code]
[list]
[*]The console shows a result summary with loaded content, warnings, and errors.
[*]If it fails, errors appear in red in the console. For the full trace, check:
[/list]
[code]
C:\Users\<yourUser>\AppData\LocalLow\Magnum Scriptum Ltd\Quasimorph\Player.log
[/code]

[h2]Upload to Steam Workshop[/h2]
[list=1]
[*]Create your mod with NBK_RedSpy's template.
[*]Create a hook for AfterConfigLoaded and call:
[/list]
[code]
QM_ImporterAPI.Services.ImporterApi.LoadModFromContext(context);
[/code]
[i]Pass context (the IModContext instance your hook receives), not the interface type itself.[/i]
[list=1]
[*]Build your mod and locate the output .dll files.
[*]Move your mod Assets folder (JSON/images/audio) to the same folder as the .dll files.
[*]Publish to Workshop and subscribe to test.
[/list]

[h2]Tips and Tricks[/h2]
[list]
[*]If a field ends with Id, it can copy properties from in-game items (icons, sprites, audio, models, etc.). Try common_knife_1 to load base knife values.
[*]Always review logs, even when the command appears successful.
[*]Use Unity External Log by NBK_RedSpy for real-time log viewing.
[*]Use give <itemId> to quickly spawn and test your custom items in-game.
[*]Use removeitem <itemId> to clean up test items without restarting.
[*]Use export-chips to reference existing datadisk and chip definitions when building chip mods.
[/list]

[h2]Support the Project[/h2]
If you want to support updates and future tools:

[url=https://ko-fi.com/crynano]Support me on Ko-fi[/url]

[h2]Special Thanks[/h2]
[list]
[*]Raigir (incredible designer)
[*]Lychantiure (awesome artist)
[*]NBK_RedSpy (incredible modder and template creator)
[/list]

Created by [url=https://github.com/Crynano]Crynano[/url]