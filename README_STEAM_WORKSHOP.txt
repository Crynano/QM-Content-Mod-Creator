[h1]Content Mod Creator - Weapon and Item Importer API[/h1]

Content Mod Creator lets you create Quasimorph weapon and item mods without dealing with repetitive setup code.
It handles folders, settings, image importing, audio importing, and configuration loading so you can focus on making content.

[h2]Quick Commands[/h2]

- Create a new mod template
[code]create-mod "PathToAFolder"[/code] 

- Import your mod into the game
[code]import-mod "PathToAFolder"[/code]

- Export 250+ base game weapons for reference
[code]export-weapons "PathToAFolder"[/code]


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
[*]If you see Imported Mod Successfully, your mod loaded correctly.
[*]If it fails, check:
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
QM_ImporterAPI.Services.ImporterApi.LoadModFromContext(IModContext);
[/code]
[list=1]
[*]Build your mod and locate the output .dll files
[*]Move your mod Assets folder (JSON/images/audio) to the same folder as the .dll files
[*]Publish to Workshop and subscribe to test.
[/list]

[h2]Tips and Tricks[/h2]
[list]
[*]If a field ends with Id, it can copy properties from in-game items (icons, sprites, audio, models, etc.). Try common_knife_1 to try it with the base game knife.
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