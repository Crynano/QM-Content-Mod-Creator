using MGSC;
using QM_ImporterAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace QM_ImporterAPI.Commands.Create
{
    [ConsoleCommand(new string[] { "create-projectile-example", "api-create-projectile-example" })]
    public class CreateProjectileExampleCommand 
    {
        public static string Help(string command, bool verbose)
        {
            return "Creates an example flamethrower projectile view JSON. Syntax: create-projectile-example <folder-path>";
        }

        public string Execute(string[] tokens)
        {
            try
            {
                if (tokens.Length == 0 || string.IsNullOrEmpty(tokens[0]))
                {
                    return "<color=red>ERROR: </color>No folder path provided. Syntax: create-projectile-example <folder-path>";
                }

                var providedPath = tokens[0];

                if (!Path.IsPathRooted(providedPath))
                {
                    return "<color=red>ERROR: </color>Provided path must be an absolute path.";
                }
                else if (!Directory.Exists(providedPath))
                {
                    return "<color=red>ERROR: </color>Provided path does not exist.";
                }

                var folder = ModCreator.CreateProjectileExample(providedPath);

                return $"<color=green>Created example projectile at \"{folder}\".</color>";
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
