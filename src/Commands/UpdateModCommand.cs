using MGSC;
using QM_ImporterAPI.Services;
using QM_ImporterAPI.Services.Helpers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QM_ImporterAPI.Commands
{
    [ConsoleCommand(new string[] { "update-mod", "api-update-mod" })]
    public class UpdateModCommand
    {
        public static string Help(string command, bool verbose)
        {
            return "updates files with new properties and properties for a mod using the Importer API. Syntax: update-mod <folder-path>";
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

                var errorMessage = Helper.ValidatePath(providedPath);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return errorMessage;
                }

                ModLoader.UpdateMod(providedPath);

                return $"<color=green>Updated files at \"{providedPath}\".</color>";
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