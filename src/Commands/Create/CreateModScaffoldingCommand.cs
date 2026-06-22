using MGSC;
using QM_ImporterAPI.Services;
using QM_ImporterAPI.Services.Helpers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QM_ImporterAPI.Commands.Create
{
    [ConsoleCommand(new string[] { "create-mod", "api-create-mod" })]
    public class CreateModScaffoldingCommand
    {
        public static string Help(string command, bool verbose)
        {
            return "Creates folders and example files to start creating a content mod using the Importer API. Syntax: create-mod <folder-path>";
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

                ModCreator.CreateExampleMod(providedPath);

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