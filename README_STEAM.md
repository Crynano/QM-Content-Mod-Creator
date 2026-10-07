[h1]Content Mod Creator - New Weapon and Item Importer API[/h1]

[url=https://ko-fi.com/crynano]Support on Ko-fi[/url]
[url=https://github.com/Crynano]Author: Crynano[/url]

Content Mod Creator helps you create Quasimorph weapon and item mods without having to touch code!
It's a straight upgrade from the Weapon and Item Importer API I've previously developed.
It handles folder creation, settings, image, audio, configuration and validations so you can focus on building stuff

If anything you've created fails, there's a detailed result summary printed in the console, and errors are logged in [i]Player.log[/i] for further inspection.

If you need any help, feel free to ask in the Quasimorph modding community on Discord or Steam.

[h2]Table of Contents[/h2]
[list]
[*]Features
[*]Quick Commands
[*]Create a Mod
[*]Import a Mod
[*]Upload to Steam Workshop
[*]Quick Guide
[*]Tips and Tricks
[*]Restrictions
[*]Troubleshooting
[*]Changelog
[*]Support
[*]Special Thanks
[*]Other Mods
[/list]

[h2]Features[/h2]
[list]
[*]Creates mod folder structure and settings for you.
[*]Imports JSON, images, and audio assets.
[*]Supports weapons, armor, ammo, firemodes, explosions, consumables, traits, datadisks, and implants.
[*]Supports grenades, trash items, custom mercenary class and mercenary profile mods.
[*]Reduces repetitive setup so you can iterate faster.
[*]Works through simple in-game console commands.
[/list]

[h2]Quick Commands[/h2]

[h3]Create[/h3]
[table]
[tr]
[td]Command
[/td]
[td]What it does
[/td]
[/tr]
[tr]
[td][i]create-mod "PathToAFolder"[/i]
[/td]
[td]Creates a full mod template folder (all content types)
[/td]
[/tr]
[tr]
[td][i]create-weapon-mod "PathToAFolder"[/i]
[/td]
[td]Creates a weapon-focused mod template folder
[/td]
[/tr]
[tr]
[td][i]create-consumable-mod "PathToAFolder"[/i]
[/td]
[td]Creates a consumable-focused mod template folder (includes trash items)
[/td]
[/tr]
[tr]
[td][i]create-grenade-mod "PathToAFolder"[/i]
[/td]
[td]Creates a grenade-focused mod template folder
[/td]
[/tr]
[tr]
[td][i]create-merc-mod "PathToAFolder"[/i]
[/td]
[td]Creates a mercenary class mod template folder
[/td]
[/tr]
[tr]
[td][i]create-trait-mod "PathToAFolder"[/i]
[/td]
[td]Creates a trait mod template folder
[/td]
[/tr]
[/table]

[h3]Import & Manage[/h3]
[table]
[tr]
[td]Command
[/td]
[td]What it does
[/td]
[/tr]
[tr]
[td][i]import-mod "PathToAFolder"[/i]
[/td]
[td]Imports your mod into the game
[/td]
[/tr]
[tr]
[td][i]update-mod "PathToAFolder"[/i]
[/td]
[td]Updates mod files with new properties
[/td]
[/tr]
[tr]
[td][i]migrate-old-mod "PathToAFolder"[/i]
[/td]
[td]Migrates old weapon records to the new format
[/td]
[/tr]
[tr]
[td][i]give <itemId> [amount] [cargoIndex][/i]
[/td]
[td]Spawns items on the floor or in ship cargo (default amount: 1)
[/td]
[/tr]
[tr]
[td][i]removeitem <itemId>[/i]
[/td]
[td]Removes all instances of a specific item from the savegame
[/td]
[/tr]
[/table]

[h3]Export[/h3]
[table]
[tr]
[td]Command
[/td]
[td]What it does
[/td]
[/tr]
[tr]
[td][i]export-weapons "PathToAFolder"[/i]
[/td]
[td]Exports 250+ in-game weapons for reference
[/td]
[/tr]
[tr]
[td][i]export-armor "PathToAFolder"[/i]
[/td]
[td]Exports in-game armor records for reference
[/td]
[/tr]
[tr]
[td][i]export-chips "PathToAFolder"[/i]
[/td]
[td]Exports all in-game item chips (datadisks) for reference
[/td]
[/tr]
[tr]
[td][i]export-mercenaryclass "PathToAFolder"[/i]
[/td]
[td]Exports in-game mercenary classes for reference
[/td]
[/tr]
[tr]
[td][i]export-mercenaryprofile "PathToAFolder"[/i]
[/td]
[td]Exports in-game mercenary profiles for reference
[/td]
[/tr]
[/table]

All [i]create-*[/i] and [i]import-mod[/i]/[i]update-mod[/i] commands also have an [i]api-[/i] prefixed alias (e.g. [i]api-create-mod[/i]).

[h2]Create a Mod from Scratch[/h2]
[olist]
[*]Install Content Mod Creator
[*]Install a developer console mod.
[*]Start Quasimorph.
[*]In the main menu, open developer console with [i]~[/i] (key left of [i]1[/i]).
[*]Run:
[/olist]
[code]
create-mod "C:/Temp/Mod"
[/code]
[olist]6"
[*]Navigate to that folder and edit the generated files.
[/olist]

[h2]Import a Mod[/h2]
[olist]
[*]Install Content Mod Creator
[*]Install a developer console mod.
[*]Start Quasimorph.
[*]In the main menu, open developer console with [i]~[/i].
[*]Run:
[/olist]
[code]
import-mod "C:/Temp/Mod"
[/code]
[olist]6"
[*]After import, the console shows a result summary with execution time, a list of loaded content, any warnings, and errors.
[*]If import fails, errors appear in red directly in the developer console. For the full trace, check [i]Player.log[/i]:
[/olist]
[code]
C:\Users\<yourUser>\AppData\LocalLow\Magnum Scriptum Ltd\Quasimorph\Player.log
[/code]

[h2]Upload to Steam Workshop[/h2]
[olist]
[*]Create your mod using NBK_RedSpy's template.
[*]Add a project reference to [i]QM_ImporterAPI.dll[/i], located at:
[/olist]
[code]
SteamLibrary\steamapps\workshop\content\2059170\3671320495\QM_ImporterAPI.dll
[/code]
[olist]3"
[*]In code, create a hook for [i]AfterConfigLoaded[/i] and call:
[/olist]
[code]
QM_ImporterAPI.Services.ImporterApi.LoadModFromContext(context);
[/code]
[quote]
Pass [i]context[/i] (the [i]IModContext[/i] instance your hook receives), not the interface type itself.
[/quote]
[olist]4"
[*]Build your mod and locate the output [i].dll[/i].
[*]Move your mod [i]Assets[/i] folder (JSON, images, audio, etc.) into the same folder as the [i].dll[/i].
[*]Publish to Workshop and subscribe to verify it loads correctly.
[/olist]
[quote]
This is a concise workflow summary. For full walkthroughs, check the Quasimorph Discord and Steam Guides.
[/quote]

[h2]Quick Guide[/h2]

[h3]Mod[/h3]
[list]
[*]You can create as many JSONs as you need, separate localization, crafting recipes or faction rewards at your will.
[*]There is no required nor fixed naming, organize folders as you wish.
[*]All config (records and descriptors) file extensions must be .json to be taken into account.
[*]Weapon ID and Descriptor ID must match for the weapon to be loaded. Same rule applies to all other configs except for crafting, which is OutputItem.
[/list]

[h3]Records[/h3]
[list]
[*]TransformationRecords are what you could get when disassembling/dismantling the weapon.
[*]ItemProduceReceipt or Crafting Recipes, define costs and time to craft and upgrade the item in the spaceship.
[*]FactionRewards define which faction and at what level the item will be given as reward.
[/list]

[h3]Restrictions[/h3]
[list]
[*][b]Requires the Quasimorph Beta Branch.[/b] This branch does not run on the stable branch.
[*]You can't add custom recipes without adding a weapon first. It will be added in the future.
[*]Custom models can't be added unless bundled in a Unity Assetbundle file. You can find tutorials online explaining this process. Requires Unity Engine installed but no prior knowledge of it.
[/list]

[h2]Tips and Tricks[/h2]
[list]
[*]If a variable ends with [i]Id[/i], it can copy properties from an in-game item (icons, sprites, audio, models, and more).
[*]Try [i]common_knife_1[/i] to load the base knife values quickly.
[*]Always review logs, even if the console command does not show an error.
[*]For live logs, use the Unity External Log mod by NBK_RedSpy.
[*]To inspect all base game weapons:
[/list]
[code]
export-weapons "C:/Temp/WeaponDump"
[/code]
[list]
[*]To inspect all base game item chips and datadisks:
[/list]
[code]
export-chips "C:/Temp/ChipDump"
[/code]
[list]
[*]Use [i]give <itemId>[/i] to quickly spawn and test your custom items in-game.
[*]Use [i]removeitem <itemId>[/i] to clean up test items from a save without restarting.
[/list]

[h3]Crafting Recipes[/h3]
[list]
[*]Crafting recipes can be finicky with ongoing saves. If a newly added recipe does not appear in-game, start a new game to verify it works.
[*]In a crafting recipe, the [i]Id[/i] field is unused. Use [i]OutputItem[/i] to specify what the recipe produces.
[*][i]ModifyItemsGrades[/i] defines the [b]total[/b] number of each chip needed to reach the maximum upgrade level. For example:
[/list]
[code]
"ModifyItemsGrades": {
  "itemChip": 7,
  "mediumItemChip": 15
},
"ModifyLevelLimit": 15
[/code]

This means 1 Medium Item Chip per upgrade level, and 1 Item Chip every 2 upgrade levels.

[h3]Item Chips[/h3]
[list]
[*]Item chip lists are [b]merged[/b], not replaced. You only need to list the items you want to add — existing chip contents are preserved automatically.
[*]You can find base game chip definitions in [i]config_items[/i] in the game data files, useful as a reference when building chip modifications.
[/list]

[h3]Creating New Chips[/h3]
[list]
[*]To create a new chip, define a chip with a brand new unique ID. It can be added to faction rewards via the mod.
[*]New chips won't have a custom sprite unless you provide one. A simple starting approach is to add them as faction rewards.
[/list]

[h3]Implants[/h3]
[list]
[*]Implants use [i]CustomImplantDescriptor[/i], which extends [i]CustomItemContentDescriptor[/i].
[*]You can provide a [i]UseSoundPath[/i] pointing to an audio file played when the implant activates.
[/list]

[h3]Datadisks[/h3]
[list]
[*]Datadisks use [i]CustomDatadiskDescriptor[/i]. Provide icon, small icon, and shadow sprite paths via [i]ImageProperties[/i].
[*]Use [i]export-chips[/i] to reference existing datadisk definitions.
[/list]

[h3]Custom Wound Slots[/h3]
[list]
[*][i]CustomWoundSlotDescriptor[/i] lets you define custom wound slots for mercenary classes.
[*]Set [i]SlotPosition[/i] and provide icon paths for each wound state: [i]NormalIconPath[/i], [i]WoundedIconPath[/i], [i]FixatedIconPath[/i], and [i]AmputatedIconPath[/i].
[/list]

[h3]Traits[/h3]
[list]
[*]Use [i]create-trait-mod[/i] to scaffold a trait mod.
[*]Trait IDs must be unique. Reference existing traits via [i]export[/i] commands for formatting guidance.
[/list]

[h3]Mercenary Classes[/h3]
[list]
[*]Use [i]create-merc-mod[/i] to scaffold a mercenary class mod.
[*]Mercenary mods support custom wound slots via [i]CustomWoundSlotDescriptor[/i].
[/list]

[h3]Updating and Migrating Mods[/h3]
[list]
[*]Use [i]update-mod[/i] to add newly introduced JSON properties to existing mod files without recreating them.
[*]Use [i]migrate-old-mod[/i] if you have mod files created with the original Weapon and Item Importer; this converts them to the current format.
[/list]

[h3]Mod Organization[/h3]
[list]
[*]Consider splitting content into separate mods per faction or theme. There is no built-in way to toggle individual elements within a single mod, so smaller focused mods give users more control over what they load.
[/list]

[h2]Troubleshooting[/h2]
[list]
[*]Item does not load even when command reports success:
Check the developer console and [i]Player.log[/i] for hidden exceptions and verify JSON field names. Every import-mod process prints a result summary with errors and failed steps.
[*]Errors during import appear in red directly in the developer console, not just in [i]Player.log[/i]. If you see a red message after running [i]import-mod[/i], that is your first signal something went wrong.
[*]Missing images or audio:
Verify file names, paths, and that files exist under your mod [i]Assets[/i] folder.

[h2]Changelog[/h2]
[*][b]Trash[/b]: Added trash item support ([i]CustomTrashDescriptor[/i]), included in the consumable template.
[*][b]Consumables[/b]: Updated consumable creation and loading.
[*][b]Mercenaries[/b]: Added mercenary classes and mercenary profiles, with [i]create-merc-mod[/i], [i]export-mercenaryclass[/i] and [i]export-mercenaryprofile[/i].
[*][b]Grenades[/b]: Added grenade records and descriptors, with [i]create-grenade-mod[/i].
[*][b]Import results[/b]: Improved import result summaries, merging warnings and errors across loaders.
[*][b]Stability[/b]: Fixed an exception when printing errors while the game is loading, and improved log messages and error details (e.g. muzzle loading).
[*][b]Crafting[/b]: Recipes now check that items exist in-game before being added.
[*][b]Icons[/b]: Tooltip icons and trait icons can be added or replaced.
[*][b]Commands[/b]: Renamed the migrate command to [i]migrate-old-mod[/i] and cleaned up commands.
[/list]

[h2]Support[/h2]

If this project helps your workflow and you want to support updates:
[list]
[*][url=https://ko-fi.com/crynano]Support Content Mod Creator on Ko-fi[/url]
[/list]

[h2]Special Thanks[/h2]
[list]
[*]Raigir (incredible designer)
[*]Lychantiure (awesome artist)
[*]NBK_RedSpy (god-tier modder and template creator)
[/list]

Mod created by [url=https://github.com/Crynano]Crynano[/url].
Feel free to ask in the Quasimorph modding community if you have questions.

[h2]My Other Quasimorph Mods[/h2]
[list]
[*]Mod Configuration Menu
[*]Expanded Faction Arsenal (EFA)
[*]Display Movement Speed UI
[*]Original Item and Weapon Importer
[*]Cyberpunk 2077 Rebel
[/list]
