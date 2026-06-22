using MGSC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Services.Cleanup
{
    public static class CleanupSystem 
    {
        public static string PerformCleanupWithidList(State state, List<string> idList)
        {
            // Check if user is ingame.
            string resultMessage = "";
            Logger.SetConfig("QM_WeaponImporter");
            Logger.SetContext("PerformCleanupWithidList()");
            try
            {
                // Execute cleanup.
                // Save game.
                // Exit to main menu.
                CleanObsoleteProjects(state, idList, true);
                resultMessage = $"Cleanup performed correctly!";
            }
            catch (Exception e)
            {
                Logger.LogError($"PerformCleanupWithidList(): Error when performing cleanup with idList {idList}.\n{e.Message}\n{e.StackTrace}\n{e.Source}");
                resultMessage = $"Error occurred while cleaning references with idList: {idList}";
            }
            finally
            {
                Logger.FlushAdditive();
                Logger.ClearContext();
            }
            return resultMessage;
        }
        
        internal static void CleanObsoleteProjects(State state, List<string> idList, bool cleanProjects = true)
        {
            Logger.LogDebug($"CleanObsoleteProjects");
            List<string> idsToRemove = new List<string>();

            var listMagnumCargo = CleanupMagnumCargo(state, idList);
            var listMissionRewards = CleanupMissionRewards(state, idList);
            var listStationItems = CleanStationInternalStorage(state, idList);
            var listMercenariesCargo = CleanupMercenariesCargo(state, idList);
            var listMagnumDepartmentsData = CleanupItemsMagnumDepartments(state, idList);
            var listCurrentCrafts = CleanupCraftingProjects(state, idList);
            
            Logger.LogDebug($"Lists count:");
            Logger.LogDebug($"\t listMagnumCargo {listMagnumCargo.Count}");
            Logger.LogDebug($"\t listMissionRewards {listMissionRewards.Count}");
            Logger.LogDebug($"\t listStationItems {listStationItems.Count}");
            Logger.LogDebug($"\t listMercenariesCargo {listMercenariesCargo.Count}");
            Logger.LogDebug($"\t listMagnumDepartmentsData {listMagnumDepartmentsData.Count}");
            Logger.LogDebug($"\t listCurrentCrafts {listCurrentCrafts.Count}");
            
            // Dedupe? There are not many entries anyway.
            idsToRemove.AddRange(listMagnumCargo);
            idsToRemove.AddRange(listMissionRewards);
            idsToRemove.AddRange(listStationItems);
            idsToRemove.AddRange(listMercenariesCargo);
            idsToRemove.AddRange(listMagnumDepartmentsData);
            idsToRemove.AddRange(listCurrentCrafts);
            
            CleanupMagnumProjects(state);

            Logger.LogDebug($"\t IDs to remove: {idsToRemove.Count}");
        }

        internal static List<string> CleanupCraftingProjects(State state, List<string> idList)
        {
            List<string> items = new List<string>();
            
            Logger.LogDebug($"[CleanupCraftingProjects]");
            
            // magnum Cargo
            var magnumCargo = state.Get<MagnumCargo>();
            var production = magnumCargo.ItemProduceOrders;
            for (int i = 0; i < production.Count; i++)
            {
                var currentList = production[i];
                foreach (ProduceOrder order in currentList.ToList())
                {
                    // Check all the orders.
                    if (idList.Any(id => id.Contains(order.OrderId)))
                    {
                        ItemProductionSystem.CancelProduction(magnumCargo, state.Get<SpaceTime>(), i, order);
                        items.Add(order.OrderId);
                    }
                }
            }

            return items;
        }

        internal static List<string> CleanItemsInAutonomousCapsuleDepartment(AutonomousCapsuleDepartment department, List<string> idList)
        {
            List<string> items = new List<string>();

            Logger.LogDebug("CleanItemsInAutonomousCapsuleDepartment");

            if (department.CapsuleStorage != null && department.CapsuleStorage.Items != null)
            {
                //items.AddRange(RecoverCleanupItems(department.CapsuleStorage.Items, idList));
                items.AddRange(CleanItemStorage(department.CapsuleStorage, idList));
            }

            return items;
        }

        internal static List<string> CleanItemsInShuttleCargoDepartment(ShuttleCargoDepartment department, List<string> idList)
        {
            List<string> items = new List<string>();

            Logger.LogDebug("CleanItemsInShuttleCargoDepartment");

            if (department.ShuttleCargo != null && department.ShuttleCargo.Items != null)
            {
                items.AddRange(CleanItemStorage(department.ShuttleCargo, idList));
            }

            return items;
        }

        internal static List<string> CleanItemsInTradeShuttleDepartment(TradeShuttleDepartment department, List<string> idList)
        {
            List<string> items = new List<string>();

            Logger.LogDebug("CleanItemsInTradeShuttleDepartment");

            //if (department.ResultStorage != null && department.ResultStorage.Items != null)
            //{
            //    items.AddRange(CleanItemStorage(department.ResultStorage, idList));
            //}

            if (department.TradeShuttleStorage != null && department.TradeShuttleStorage.Items != null)
            {
                items.AddRange(CleanItemStorage(department.TradeShuttleStorage, idList));
            }

            return items;
        }


        // private static List<string> CleanupItemsBarehandWeapons()
        // {
        //     List<string> items = new List<string>();
        //
        //     foreach (var rec in RecordCollection.WoundSlotRecords)
        //     {
        //         if (rec.Value.BareHandWeapon != string.Empty)
        //         {
        //             items.Add(rec.Value.BareHandWeapon);
        //         }
        //     }
        //
        //     return items;
        // }

        private static List<string> CleanupItemsItemsOnFloor(State state, List<string> idList)
        {
            ItemsOnFloor itemsOnFloor = state.Get<ItemsOnFloor>();
            List<string> items = new List<string>();

            Logger.LogDebug($"[CleanupItemsItemsOnFloor]");
            Logger.LogDebug($"[CleanupItemsItemsOnFloor] itemsOnFloor null {itemsOnFloor == null}");

            if (itemsOnFloor != null)
            {
                foreach (var value in itemsOnFloor.Values)
                {
                    if (value.Storage != null)
                    {
                        items.AddRange(RecoverCleanupItems(value.Storage.Items, idList));
                    }

                }
            }

            return items;
        }

        private static List<string> CleanupItemsMapObstacles(State state, List<string> idList)
        {
            MapObstacles obstacles = state.Get<MapObstacles>();
            List<string> items = new List<string>();

            Logger.LogDebug($"[CleanupItemsMapObstacles]");
            Logger.LogDebug($"[CleanupItemsMapObstacles] obstacles null {obstacles == null}");

            if (obstacles != null)
            {
                foreach (var obstacle in obstacles.Obstacles)
                {
                    foreach (var comp in obstacle._comps)
                    {
                        var corpseStorage = comp as CorpseStorage;
                        if (corpseStorage != null && corpseStorage._creatureData != null && corpseStorage._creatureData.Inventory != null)
                        {
                            foreach (ItemStorage storage in corpseStorage._creatureData.Inventory.AllContainers)
                            {
                                items.AddRange(RecoverCleanupItems(storage.Items, idList));
                            }

                            var store = comp as Store;

                            if (store != null && store.storage != null && store.storage.Items != null)
                            {
                                items.AddRange(RecoverCleanupItems(store.storage.Items, idList));
                            }
                        }
                    }
                }
            }

            return items;
        }

        internal static List<string> CleanupCreatureData(State state, List<string> idList)
        {
            Creatures creatures = state.Get<Creatures>();
            List<string> items = new List<string>();

            Logger.LogDebug($"[CleanupCreatureData]");
            Logger.LogDebug($"[CleanupCreatureData] creatures null {creatures == null}");

            if (creatures != null)
            {
                Logger.LogDebug($"player creature {creatures.Player.CreatureData.UniqueId}");

                // Cleanup player data in raid
                foreach (ItemStorage storage in creatures.Player.CreatureData.Inventory.AllContainers)
                {
                    items.AddRange(RecoverCleanupItems(storage.Items, idList));
                }

                foreach (ItemStorage storage in creatures.Player.CreatureData.Inventory.WeaponSlots)
                {
                    items.AddRange(RecoverCleanupItems(storage.Items, idList));
                }

                // Cleanup monsters
                foreach (var creature in creatures.Monsters)
                {
                    var creatureData = creature.CreatureData;
                    if (creatureData == null)
                    {
                        continue;
                    }

                    Logger.LogDebug($"monster creature {creatureData.UniqueId}");

                    foreach (ItemStorage storage in creatureData.Inventory.AllContainers)
                    {
                        items.AddRange(RecoverCleanupItems(storage.Items, idList));
                    }

                    // Part of all containers
                    //foreach (ItemStorage storage in creatureData.Inventory.Storages)
                    //{
                    //    items.AddRange(CleanupPickupItem(storage.Items));
                    //}

                    //foreach (ItemStorage storage in creatureData.Inventory.Slots)
                    //{
                    //    items.AddRange(CleanupPickupItem(storage.Items));
                    //}

                    foreach (ItemStorage storage in creatureData.Inventory.WeaponSlots)
                    {
                        items.AddRange(RecoverCleanupItems(storage.Items, idList));
                    }
                }
            }

            return items;
        }

        internal static List<string> CleanupItemsMagnumDepartments(State state, List<string> idList)
        {
            List<string> items = new List<string>();

            MagnumProgression magnumSpaceship = state.Get<MagnumProgression>();

            Logger.LogDebug($"[CleanupItemsMagnumDepartments]");
            Logger.LogDebug($"[CleanupItemsMagnumDepartments] magnumSpaceship null {magnumSpaceship == null}");

            foreach (var department in magnumSpaceship.Departments)
            {
                switch (department._departmentId)
                {
                    case "autonomcapsule_department":
                        items.AddRange(CleanItemsInAutonomousCapsuleDepartment(department as AutonomousCapsuleDepartment, idList));
                        break;
                    case "cargoshuttle_department":
                        items.AddRange(CleanItemsInShuttleCargoDepartment(department as ShuttleCargoDepartment, idList));
                        break;
                    case "tradeshuttle_department":
                        items.AddRange(CleanItemsInTradeShuttleDepartment(department as TradeShuttleDepartment, idList));
                        break;
                }
            }

            return items;
        }

        internal static List<string> CleanStationInternalStorage(State state, List<string> idList)
        {
            Stations stations = state.Get<Stations>();
            List<string> items = new List<string>();

            Logger.LogDebug($"[CleanStationInternalStorage]");
            Logger.LogDebug($"[CleanStationInternalStorage] stations null {stations == null}");

            if (stations != null)
            {
                foreach (var station in stations.Values)
                {
                    if (station.InternalStorage != null)
                    {
                        //items.AddRange(RecoverCleanupItems(station.InternalStorage.Items, idList));
                        items.AddRange(CleanItemStorage(station.InternalStorage, idList));
                    }

                    if (station.Stash != null)
                    {
                        items.AddRange(CleanItemStorage(station.Stash, idList));
                    }

                }
            }

            return items;
        }

        // internal static List<string> CleanupItemsOnFloor(State state, List<string> idList)
        // {
        //     ItemsOnFloor itemsOnFloor = state.Get<ItemsOnFloor>();
        //     List<string> items = new List<string>();
        //
        //     Logger.LogDebug($"[CleanupItemsOnFloor]");
        //     Logger.LogDebug($"[CleanupItemsOnFloor] itemsOnFloor null {itemsOnFloor == null}");
        //
        //     if (itemsOnFloor != null)
        //     {
        //         foreach (var itemOnFloor in itemsOnFloor.Values)
        //         {
        //             if (itemOnFloor.Storage != null && itemOnFloor.Storage.Items != null)
        //             {
        //                 items.AddRange(RecoverCleanupItems(itemOnFloor.Storage.Items, idList));
        //             }
        //         }
        //     }
        //
        //     return items;
        // }

        internal static List<string> CleanupMagnumCargo(State state, List<string> idList)
        {
            MagnumCargo magnumCargo = state.Get<MagnumCargo>();
            List<string> items = new List<string>();

            Logger.LogDebug($"[CleanupMercenariesCargo]");

            if (magnumCargo != null)
            {
                foreach (var cargo in magnumCargo.ShipCargo)
                {
                    items.AddRange(CleanItemStorage(cargo, idList));
                }

                items.AddRange(CleanItemStorage(magnumCargo.RecyclingStorage, idList));
                
                Logger.LogDebug($"[CleanupMercenariesCargo] Cleaning UnlockedProductionItems");
                
                foreach (var productionId in magnumCargo.UnlockedProductionItems.ToList())
                {
                    if (idList.Any(id => id.Contains(productionId)))
                    {
                        magnumCargo.UnlockedProductionItems.Remove(productionId);
                        items.Add(productionId);   
                    }
                }
            }
            return items;
        }
        

        internal static void CleanupMagnumProjects(State state)
        {
            // Get the current game time (you can also use Time.time or Time.unscaledTime depending on your need)
            MagnumProjects magnumProjects = state.Get<MagnumProjects>();

            Logger.LogDebug($"[CleanupMagnumProjects]");
            Logger.LogDebug($"[CleanupMagnumProjects] magnumProjects null {magnumProjects == null}");

            if (magnumProjects != null)
            {
                Logger.LogDebug($"magnumProjects != null");
                foreach (var project in magnumProjects.Values.ToList()) // Using ToList() to avoid modification during iteration
                {
                    Logger.LogDebug($"\t checking project {project.DevelopId}");
                    magnumProjects.Values.Remove(project); // Remove the item if it doesn't meet the condition
                }
            }
        }

        internal static List<string> CleanupMercenariesCargo(State state, List<string> idList)
        {
            Mercenaries mercenaries = state.Get<Mercenaries>();
            List<string> items = new List<string>();

            Logger.LogDebug($"[CleanupMercenariesCargo]");
            Logger.LogDebug($"[CleanupMercenariesCargo] mercenaries null {mercenaries == null}");

            if (mercenaries != null)
            {
                Logger.LogDebug("mercenaries != null");

                foreach (var merc in mercenaries.Values)
                {
                    Logger.LogDebug($"merc {merc.ProfileId}");

                    foreach (ItemStorage storage in merc.CreatureData.Inventory.AllContainers)
                    {
                        //Logger.LogDebug($"storage {storage}");

                        //items.AddRange(CleanupPickupItem(storage.Items, idList));
                        items.AddRange(CleanItemStorage(storage, idList));
                    }

                    foreach (ItemStorage storage in merc.CreatureData.Inventory.WeaponSlots)
                    {
                        items.AddRange(CleanItemStorage(storage, idList));
                    }
                }
            }

            return items;
        }

        internal static List<string> CleanupMissionRewards(State state, List<string> idList)
        {
            Missions missions = state.Get<Missions>();
            List<string> items = new List<string>();

            Logger.LogDebug($"[CleanupMissionRewards]");
            Logger.LogDebug($"[CleanupMissionRewards] missions null {missions == null}");

            if (missions != null)
            {
                foreach (var mission in missions.Values)
                {
                    //items.AddRange(RecoverCleanupItems(misson.RewardItems, idList));
                    foreach (var rewardItem in mission.RewardItems.ToList())
                    {
                        if (idList.Any(id => id.Contains(rewardItem.Id)))
                        {
                            mission.RewardItems.Remove(rewardItem);
                            items.Add(rewardItem.Id);
                        }
                    }
                    //items.AddRange(RecoverCleanupItems(misson.RewardItemsExample, idList));
                    foreach (var rewardItem in mission.RewardItemsExample.ToList())
                    {
                        if (idList.Any(id => id.Contains(rewardItem.Id)))
                        {
                            mission.RewardItemsExample.Remove(rewardItem);
                            items.Add(rewardItem.Id);
                        }
                    }
                }

                foreach (var mission in missions.Reversed)
                {
                    //items.AddRange(RecoverCleanupItems(misson.RewardItems, idList));
                    foreach (var rewardItem in mission.RewardItems.ToList())
                    {
                        if (idList.Any(id => id.Contains(rewardItem.Id)))
                        {
                            mission.RewardItems.Remove(rewardItem);
                            items.Add(rewardItem.Id);
                        }
                    }
                    //items.AddRange(RecoverCleanupItems(misson.RewardItemsExample, idList));
                    foreach (var rewardItem in mission.RewardItemsExample.ToList())
                    {
                        if (idList.Any(id => id.Contains(rewardItem.Id)))
                        {
                            mission.RewardItemsExample.Remove(rewardItem);
                            items.Add(rewardItem.Id);
                        }
                    }
                }
            }

            return items;
        }
        
        internal static List<string> CleanItemStorage(ItemStorage storage, List<string> idList)
        {
            var cleanup = RecoverCleanupItems(storage.Items, idList);
            foreach (string itemID in cleanup)
            {
                storage.RemoveSpecificItem(itemID, 999);
            }
            return cleanup;
        }
        
        internal static List<string> RecoverCleanupItems(List<BasePickupItem> basePickupItemsList, List<string> idList)
        {
            //Logger.LogDebug($"CleanupPickupItem");

            List<string> itemsToReturn = new List<string>();

            foreach (PickupItem item in basePickupItemsList)
            {
                if (idList.Any(id => id.Contains(item.Id)))
                {
                    Logger.LogDebug($"CleanupPickupItem REMOVING item: {item.Id} - {MGSC.Localization.Get($"item.{item.Id}.name")}");
                    itemsToReturn.Add(item.Id);
                }
            }

            return itemsToReturn;
        }
    }
}