using MGSC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Commands.General
{
    [ConsoleCommand(new string[] { "removeitem" })]
    public class RemoveItemCommand
    {
        public static string Help(string command, bool verbose)
        {
            return "Remove all instances of this item from the savegame. Syntax: removeitem <itemId>";
        }

        public string Execute(string[] tokens)
        {
            try
            {
                if (SingletonMonoBehaviour<SpaceGameMode>.Instance == null) return "ERROR: Command can only be executed while in Space";

                string itemID;
                if (tokens.Length < 1) return "No ID has been provided";

                itemID = tokens[0];
                if (!Data.Items.Ids.Contains(itemID)) return "ERROR: ID provided does not exist.";

                // Remove
                var result = Services.Cleanup.CleanupSystem.PerformCleanupWithidList(
                    SingletonMonoBehaviour<SpaceGameMode>.Instance._state,
                    new List<string>() { itemID });

                if (UI.IsShowing<ArsenalScreen>())
                {
                    UI.Get<ArsenalScreen>().RefreshView();
                }

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "All instances of the item {} have been removed from the savegame.";
        }

        public static List<string> FetchAutocompleteOptions(string command, string[] tokens)
        {
            string enteredText = ((tokens.Length != 0) ? tokens[0] : "");
            //UI.Get<DevConsole>().Daemon.CommandList.Where((string name) => name.StartsWith(enteredText)).ToList();
            List<string> list = Data.Items.Ids.Where((string name) => name.Contains(enteredText)).ToList();
            if (list.Count == 0)
            {
                return null;
            }

            List<string> list2 = new List<string>();
            foreach (string item in list)
            {
                list2.Add($"{command} {item}");
            }

            return list2;
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