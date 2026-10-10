using MGSC;
using QM_ImporterAPI.Services;
using QM_ImporterAPI.Services.Helpers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QM_ImporterAPI.Commands.Create
{
    [ConsoleCommand(new string[] { "create-augmentation-mod", "api-create-augmentation-mod" })]
    public class CreateAugmentationModCommand
    {
        public static string Help(string command, bool verbose)
        {
            return "Creates folders and example files (based on an existing augmentation) to start creating an augmentation mod using the Importer API. Syntax: create-augmentation-mod <folder-path>";
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

                ModCreator.CreateAugmentationMod(providedPath);

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
