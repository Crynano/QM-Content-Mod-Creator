using MGSC;
using QM_ImporterAPI.Services;
using QM_ImporterAPI.Services.Helpers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QM_ImporterAPI.Commands.Create
{
    [ConsoleCommand(new string[] { "create-helmet-mod", "api-create-helmet-mod" })]
    public class CreateHelmetModCommand
    {
        public static string Help(string command, bool verbose)
        {
            return "Creates folders and example files (based on an existing helmet) to start creating a helmet mod using the Importer API. Syntax: create-helmet-mod <folder-path>";
        }

        public string Execute(string[] tokens)
        {
            try
            {
                var providedPath = Helper.FilterToken(tokens, 0);
                if (string.IsNullOrEmpty(providedPath))
                {
                    return "<color=red>ERROR: </color>No folder path provided.";
                }

                var isValid = Helper.ValidatePath(providedPath, out string errorMessage);
                if (!isValid)
                {
                    return $"<color=red>ERROR: </color>{errorMessage}";
                }

                ModCreator.CreateHelmetMod(providedPath);

                return $"<color=green>Created Assets folder and example files at \"{providedPath}\".</color>";
            }
            catch (Exception ex)
            {
                string msg = $"<color=red>ERROR: </color>" + ex.Message;
                Debug.LogError(ex.StackTrace);
                return msg;
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
