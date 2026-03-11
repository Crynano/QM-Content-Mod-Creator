[h1]Importer v2 - Weapon and Item Importer API[/h1]

[quote]
Importer v2 lets you create Quasimorph weapon and item mods without dealing with repetitive setup code.
It handles folders, settings, image importing, audio importing, and configuration loading so you can focus on making content.
[/quote]

[h2]Quick Commands[/h2]
[list]
[*][code]create-mod "PathToAFolder"[/code] - Create a new mod template
[*][code]import-mod "PathToAFolder"[/code] - Import your mod into the game
[*][code]export-weapons "PathToAFolder"[/code] - Export 250+ base game weapons for reference
[/list]

[h2]Create a Mod from Scratch[/h2]
[list=1]
[*]Install Importer v2 and a developer console mod.
[*]Start Quasimorph.
[*]In main menu, open the developer console with [code]~[/code] (key left of [code]1[/code]).
[*]Run:
[/list]

[code]
create-mod "C:/Temp/Mod"
[/code]

[list=1]
[*]Go to that folder and edit your generated files.
[/list]

[h2]Import a Mod[/h2]
[list=1]
[*]Open developer console with [code]~[/code].
[*]Run:
[/list]

[code]
import-mod "C:/Temp/Mod"
[/code]

[list]
[*]If you see [code]Imported Mod Successfully![/code], your mod loaded correctly.
[*]If it fails, check:
[/list]

[code]
C:\Users\<yourUser>\AppData\LocalLow\Magnum Scriptum Ltd\Quasimorph\Player.log
[/code]

[h2]Upload to Steam Workshop[/h2]
[list=1]
[*]Create your mod with NBK_RedSpy's template.
[*]Create a hook for [code]AfterConfigLoaded[/code] and call:
[/list]

[code]
QM_ImporterAPI.Services.ImporterApi.LoadModFromContext(IModContext);
[/code]

[list=1]
[*]Build your mod and locate the output [code].dll[/code].
[*]Move your mod [code]Assets[/code] folder (JSON/images/audio) to the same folder as the [code].dll[/code].
[*]Publish to Workshop and subscribe to test.
[/list]

[h2]Tips and Tricks[/h2]
[list]
[*]If a field ends with [code]Id[/code], it can copy properties from in-game items (icons, sprites, audio, models, etc.).
[*]Try [code]common_knife_1[/code] for quick base values.
[*]Always review logs, even when the command appears successful.
[*]Use Unity External Log by NBK_RedSpy for real-time log viewing.
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