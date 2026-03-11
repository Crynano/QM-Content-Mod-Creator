using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MGSC;

namespace QM_ImporterAPI.Commands
{
    [ConsoleCommand(new string[] { "give" })]
    public class GiveItemCommand
    {
        [Inject(false)] private readonly MagnumCargo _magnumCargo;

        [Inject(false, AllowNull = true)] private readonly MapGrid _mapGrid;

        [Inject(false, AllowNull = true)] private readonly Creatures _creatures;

        [Inject(false, AllowNull = true)] private readonly ItemsOnFloor _itemsOnFloor;

        public static string Help(string command, bool verbose)
        {
            return
                "Spawn X items on the floor or in ship cargo. Default amount is 1.\nSyntax: give <itemId> <itemAmount> <cargoStorageIndex>.\nPress TAB to autocomplete.";
        }

        public string Execute(string[] tokens)
        {
            try
            {
                int amountOfItems = 1;
                int cargoStorage = 0;

                if (tokens.Length > 2)
                {
                    string amountToken = tokens[2] ?? "1";
                    if (int.TryParse(amountToken, out int amountOfItemsParsed))
                    {
                        amountOfItems = Mathf.Clamp(amountOfItemsParsed, 1, 9999);
                    }
                }

                if (tokens.Length > 3)
                {
                    string cargoToken = tokens[3] ?? "1";
                    // If user wants more than one, specify
                    if (int.TryParse(cargoToken, out int cargoStorageParsed))
                    {
                        cargoStorage = Mathf.Clamp(cargoStorageParsed - 1, 0, 7);
                    }
                }
            
                bool cargoFlag = SingletonMonoBehaviour<DungeonGameMode>.Instance == null;
            
                for (int i = 0; i < amountOfItems; i++)
                {
                    BasePickupItem basePickupItem = SingletonMonoBehaviour<ItemFactory>.Instance.CreateForInventory(tokens[0]);
                    if (basePickupItem.Is<DatadiskRecord>())
                    {
                        DatadiskComponent datadiskComponent = basePickupItem.Comp<DatadiskComponent>();
                        DatadiskRecord datadiskRecord = basePickupItem.Record<DatadiskRecord>();
                        datadiskComponent.SetUnlockId(datadiskRecord.UnlockIds[UnityEngine.Random.Range(0, datadiskRecord.UnlockIds.Count)]);
                    }
                
                    if (cargoFlag)
                    {
                        _magnumCargo.ShipCargo[cargoStorage].AddItemAndReshuffleOptional(basePickupItem);
                    }
                    else
                    {
                        Player player = _creatures.Player;
                        ItemOnFloorSystem.SpawnItem(_itemsOnFloor, _mapGrid, basePickupItem, player.CreatureData.Position);
                    }
                }

                if (UI.IsShowing<ArsenalScreen>())
                {
                    UI.Get<ArsenalScreen>().RefreshView();
                }

                return (amountOfItems == 1 ? $"Added item" : $"Added {amountOfItems} items") +
                       (cargoFlag ? $" to cargo {cargoStorage + 1}" : " to the ground!");
            }
            catch (Exception exception)
            {
                string msg = $"<color=red>ERROR:</color> Coudn't add \"{tokens[0]}\".";
                Debug.LogError(exception);
                return msg;
            }
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
                // Also add its localization name.
                string locName = Localization.Get("item." + item + ".name").FirstLetterToUpperCase() ?? "???";
                list2.Add($"{command} {item} \"{locName}\"");
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