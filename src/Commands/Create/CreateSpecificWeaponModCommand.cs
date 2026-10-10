using MGSC;
using QM_ImporterAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace QM_ImporterAPI.Commands.Create
{
    [ConsoleCommand(new string[] { "create-specific-weapon-mod", "api-create-specific-weapon-mod" })]
    public class CreateSpecificWeaponModCommand
    {
        private const string SYNTAX = "Syntax: create-specific-weapon-mod <weapon-id> <folder-path>";

        public static string Help(string command, bool verbose)
        {
            return "Exports a weapon with its firemodes, default ammo, projectile and localization from the game. " + SYNTAX;
        }

        public string Execute(string[] tokens)
        {
            try
            {
                if (tokens.Length < 2 || string.IsNullOrEmpty(tokens[0]) || string.IsNullOrEmpty(tokens[1]))
                {
                    return "<color=red>ERROR: </color>Weapon id and folder path are required. " + SYNTAX;
                }

                var weaponId = tokens[0];
                var providedPath = tokens[1];

                if (!Path.IsPathRooted(providedPath))
                {
                    return "<color=red>ERROR: </color>Provided path must be an absolute path.";
                }
                else if (!Directory.Exists(providedPath))
                {
                    return "<color=red>ERROR: </color>Provided path does not exist.";
                }

                var assetsFolder = ModCreator.CreateSpecificWeaponMod(weaponId, providedPath);
                if (assetsFolder == null)
                {
                    return $"<color=red>ERROR: </color>Weapon with id \"{weaponId}\" does not exist.";
                }

                return $"<color=green>Exported weapon \"{weaponId}\" and related files to \"{assetsFolder}\".</color>";
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.StackTrace);
                return "<color=red>ERROR: </color>" + ex.Message;
            }
        }

        public static List<string> FetchAutocompleteOptions(string command, string[] tokens)
        {
            return new List<string>();
        }

        public static bool IsAvailable()
        {
            return true;
        }

        public static bool ShowInHelpAndAutocomplete()
        {
            return true;
        }
    }
}
