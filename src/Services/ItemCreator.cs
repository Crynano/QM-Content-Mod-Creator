using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Extensions.Descriptors;
using QM_ImporterAPI.Services.Extensions.Records;
using QM_ImporterAPI.Services.Helpers;
using QM_ImporterAPI.Templates;
using QM_ImporterAPI.Templates.Descriptors;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QM_ImporterAPI.Services
{
    public static class ItemCreator
    {
        private const string MELEE_WEAPON_DESCRIPTOR_KEY = "meleeweapons";
        private const string RANGED_WEAPON_DESCRIPTOR_KEY = "rangeweapons";

        public static ImportOperationResult ReplaceWeapon(WeaponRecord weapon, string assetFolderPath)
        {
            var operation = new ImportOperationResult();

            var originalWeapon = QuasimorphHelper.GetExistingItemRecord<WeaponRecord>(weapon.Id);
            if (originalWeapon is null)
            {
                operation.AddWarning($"Weapon with ID: {weapon.Id} does not exist in the game. If adding a new weapon, remember to add the descriptor too.");
            }
            else
            {
                var weaponPropertiesResult = weapon.CheckWeaponPropertiesRestrictions();
                operation.CopyMessages(weaponPropertiesResult);
                if (!weaponPropertiesResult.IsSuccess)
                {
                    return operation;
                }

                weapon.ContentDescriptor = originalWeapon.ContentDescriptor;

                var addItemResult = AddItemToGame(weapon);
                operation.Absorb(addItemResult);

                operation.ContentList.Add(weapon.Id);
                return operation;
            }

            return operation;
        }

        public static ImportOperationResult CreateWeapon(WeaponRecord weapon, CustomWeaponDescriptor weaponDescriptor, string assetFolderPath)
        {
            Logger.LogDebug($"Called {nameof(CreateWeapon)} with ID: " + weapon.Id);
            var operationResult = new ImportOperationResult();

            var weaponPropertiesResult = weapon.CheckWeaponPropertiesRestrictions();
            operationResult.CopyMessages(weaponPropertiesResult);
            if (!weaponPropertiesResult.IsSuccess)
            {
                return operationResult;
            }

            var descriptorPropertiesResult = weapon.SetDescriptorProperties(weaponDescriptor, assetFolderPath);
            operationResult.CopyMessages(descriptorPropertiesResult);
            if (!descriptorPropertiesResult.IsSuccess)
            {
                return operationResult;
            }

            var addItemResult = AddItemToGame(weapon);
            operationResult.Absorb(addItemResult);

            operationResult.ContentList.Add(weapon.Id);
            return operationResult;
        }

        public static ImportOperationResult AddAmmo(AmmoRecord ammo, CustomAmmoDescriptor customAmmoDescriptor, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();
            Logger.LogDebug($"Attempting to add ammo");
            if (ammo is null || customAmmoDescriptor is null)
            {
                operationResult.AddError("Can't add ammo because one of the parameters is null.");
                return operationResult;
            }
            Logger.LogDebug($"Called {nameof(AddAmmo)} with ID: " + ammo.Id);

            var descriptorAssignmentResult = SetAmmoDescriptorProperties(ammo, customAmmoDescriptor, assetFolderPath);
            operationResult.CopyMessages(descriptorAssignmentResult);
            if (!descriptorAssignmentResult.IsSuccess)
            {
                return operationResult;
            }

            var addItemResult = AddItemToGame(ammo);
            operationResult.Absorb(addItemResult);

            operationResult.ContentList.Add(ammo.Id);
            return operationResult;
        }

        public static ImportOperationResult AddItemTransformation(IEnumerable<ItemTransformationRecord> craftingRecords)
        {
            var operationResult = new ImportOperationResult();

            foreach (var craftingRecord in craftingRecords)
            {
                if (craftingRecord is null) continue;

                if (QuasimorphHelper.IsGameId(craftingRecord.Id))
                {
                    operationResult.AddWarning($"An item transformation with ID: [{craftingRecord.Id}] already exists. Its disassembly result will be overridden. " +
                        $"This happens because this item has a legacy file called \"MGSC.ItemTransformationRecord\" associated to it. " +
                        $"For compatibility, the old transformation data is being mapped to the new item transformation system. " +
                        $"Please update your weapon model to include the \"Disassembly\" property. " +
                        $"You can do so manually or by running the command: \"update-mod\" into your mod.");

                    var itemRecord = QuasimorphHelper.GetExistingItemRecord<ItemRecord>(craftingRecord.Id);
                    itemRecord.Disassembly = craftingRecord.OutputItems;
                }
            }

            return operationResult;
        }

        public static ImportOperationResult AddItemCraftRecipe(IEnumerable<ItemProduceReceipt> craftingRecipes)
        {
            var operationResult = new ImportOperationResult();

            foreach (var recipe in craftingRecipes)
            {
                if (recipe is null) continue;

                if (!QuasimorphHelper.IsGameId(recipe.OutputItem))
                {
                    operationResult.AddWarning($"A crafting recipe contains a non-existing game ID: [{recipe.OutputItem}].");
                    continue;
                }

                if (recipe.RequiredItems != null)
                {
                    var missingInputs = recipe.RequiredItems
                        .Where(input => !QuasimorphHelper.IsGameId(input.ItemId))
                        .Select(input => input.ItemId)
                        .ToList();

                    if (missingInputs.Any())
                    {
                        operationResult.AddWarning($"Crafting recipe for [{recipe.OutputItem}] references non-existing input items: [{string.Join(", ", missingInputs)}].");
                        continue;
                    }
                }

                var foundRecipe = Data.ProduceReceipts.FirstOrDefault(r => r.OutputItem.Equals(recipe.OutputItem));
                if (foundRecipe != null)
                {
                    Data.ProduceReceipts.Remove(foundRecipe);
                    operationResult.AddWarning($"Warning: A crafting recipe with ID: [{recipe.OutputItem}] was overriden");
                }
                Data.ProduceReceipts.Add(recipe);
            }

            return operationResult;
        }

        public static ImportOperationResult AddDatadiskItems(DatadiskRecord diskRecord, CustomDatadiskDescriptor customDatadiskDescriptor, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();
            
            var dataDiskCompositeRecord = (CompositeItemRecord)MGSC.Data.Items.GetRecord(diskRecord.Id);
            if (dataDiskCompositeRecord != null)
            {
                var dataDiskItemRecord = dataDiskCompositeRecord.GetRecord<DatadiskRecord>();

                // Add ONLY those not registered and ingame!
                // Should log all those not in game!
                var addOnlyThoseNotInChip = diskRecord.UnlockIds
                    .FindAll(id => !dataDiskItemRecord.UnlockIds.Contains(id) && QuasimorphHelper.IsGameId(id));

                var thoseNotInGame = diskRecord.UnlockIds
                    .FindAll(id => !QuasimorphHelper.IsGameId(id));

                thoseNotInGame.ForEach(id => operationResult.AddWarning($"Not adding {id} to datadisk item. Item does not exist in-game."));

                dataDiskItemRecord.UnlockIds.AddRange(addOnlyThoseNotInChip);
            }
            else
            {
                if (customDatadiskDescriptor is null)
                {
                    operationResult.AddError($"Custom datadisk descriptor for {diskRecord.Id} is null. Can't add new datadisk item without a descriptor.");
                    return operationResult;
                }
                var datadiskDescriptor = ScriptableObject.CreateInstance<DatadiskDescriptor>();
                datadiskDescriptor.LoadSprites(customDatadiskDescriptor, assetFolderPath);

                diskRecord.ContentDescriptor = datadiskDescriptor;

                var addItemOperation = AddItemToGame(diskRecord);
                operationResult.Absorb(addItemOperation);
            }

            return operationResult;
        }

        public static ImportOperationResult AddFactionRewards(FactionTemplate factionTemplate)
        {
            var operationResult = new ImportOperationResult();
            foreach (FactionReward factionReward in factionTemplate.FactionRewardList)
            {
                foreach (ContentDropRecord contentRecord in factionReward.contentRecords)
                {
                    contentRecord.ContentIds
                        .Where(contentId => !QuasimorphHelper.IsGameId(contentId))
                        .ToList()
                        .ForEach(x => operationResult.AddWarning($"Not adding {x} to faction table. Weapon does not exist."));

                    contentRecord.ContentIds = contentRecord.ContentIds
                        .Where(contentId => QuasimorphHelper.IsGameId(contentId))
                        .ToList();

                    if (contentRecord.ContentIds.Count > 0)
                    {
                        Data.FactionDrop.AddRecord(factionReward.GetTableName(), contentRecord);
                    }
                }
            }
            return operationResult;
        }

        private static ImportOperationResult AddItemToGame<TRecord>(TRecord record) where TRecord : BasePickupItemRecord
        {
            var operationResult = new ImportOperationResult();
            if (QuasimorphHelper.IsGameId(record.Id, Data.Items))
            {
                Data.Items.RemoveRecord(record.Id);
                operationResult.AddWarning($"An item with ID: \"{record.Id}\" was overriden.");
            }

            if (record is WeaponRecord weaponRecord)
            {
                string key = (weaponRecord.IsMelee ? MELEE_WEAPON_DESCRIPTOR_KEY : RANGED_WEAPON_DESCRIPTOR_KEY);
                Data.Descriptors[key].AddDescriptor(weaponRecord.Id, weaponRecord.ItemDesc);
            }
            else if (record is AmmoRecord ammoRecord)
            {
                Data.Descriptors["ammo"].AddDescriptor(ammoRecord.Id, ammoRecord.ItemDesc);
            }
            else if (record is DatadiskRecord)
            {
                Data.Descriptors["datadisks"].AddDescriptor(record.Id, record.ItemDesc);
            }
            else if (record is ConsumableRecord)
            {
                Data.Descriptors["consumables"].AddDescriptor(record.Id, record.ItemDesc);
            }
            else
            {
                operationResult.AddWarning($"Item [{record.Id}] of type {record.GetType().Name} has NOT been added to Data.Descriptors");
            }

            Logger.LogDebug($"Adding item with ID: \"{record.Id}\" of type \"{record.GetType().Name}\" to game.");
            Data.Items.AddRecord(record.Id, record);
            return operationResult;
        }

        private static ImportOperationResult SetAmmoDescriptorProperties(AmmoRecord ammoRecord, CustomAmmoDescriptor customAmmoDescriptor, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();
            var ammoDescriptor = ScriptableObject.CreateInstance<AmmoDescriptor>();
            bool skipGibs = string.IsNullOrEmpty(customAmmoDescriptor.Gibs.BulletSpritesId) && string.IsNullOrEmpty(customAmmoDescriptor.Gibs.BulletShadowsId);

            if (!skipGibs)
            {
                var gibsDescriptor = ScriptableObject.CreateInstance<GibsDescriptor>();
                if (QuasimorphHelper.IsGameId(customAmmoDescriptor.Gibs.BulletSpritesId))
                {
                    var gibsFromItem = QuasimorphHelper.GetPropertyFromItem<AmmoDescriptor>(customAmmoDescriptor.Gibs.BulletSpritesId, nameof(AmmoDescriptor.Gibs)) as GibsDescriptor;
                    if (gibsFromItem != null)
                    {
                        gibsDescriptor._normalSprites = gibsFromItem._normalSprites;
                    }
                    else
                    {
                        operationResult.AddWarning($"Unable to load gibs sprites from existing game item with ID: {customAmmoDescriptor.Gibs.BulletSpritesId}");
                    }
                }
                else
                {
                    operationResult.AddWarning($"Unable to find in-game ID \"{customAmmoDescriptor.Gibs.BulletSpritesId}\" for BulletSpritesId property for \"{ammoRecord.Id}\"");
                }

                if (QuasimorphHelper.IsGameId(customAmmoDescriptor.Gibs.BulletShadowsId))
                {
                    var gibsFromItem = QuasimorphHelper.GetPropertyFromItem<AmmoDescriptor>(customAmmoDescriptor.Gibs.BulletShadowsId, nameof(AmmoDescriptor.Gibs)) as GibsDescriptor;
                    if (gibsFromItem != null)
                    {
                        gibsDescriptor._shadowsSprites = gibsFromItem._shadowsSprites;
                    }
                    else 
                    {
                        operationResult.AddWarning($"Unable to load gibs sprites from existing game item with ID: {customAmmoDescriptor.Gibs.BulletSpritesId}");
                    }
                }
                else
                {
                    operationResult.AddWarning($"Unable to find in-game ID \"{customAmmoDescriptor.Gibs.BulletSpritesId}\" for BulletSpritesId property for \"{ammoRecord.Id}\"");
                }
                ammoDescriptor._gibs = gibsDescriptor;

                gibsDescriptor._animFramerateRange = new Vector2(customAmmoDescriptor.Gibs.AnimationFramerate, customAmmoDescriptor.Gibs.AnimationFramerate);
                gibsDescriptor._flyDurationRange = new Vector2(customAmmoDescriptor.Gibs.FlightDurationMsMin, customAmmoDescriptor.Gibs.FlightDurationMsMax);
            }

            ammoDescriptor.LoadSprites(customAmmoDescriptor, assetFolderPath);
            
            ammoRecord.ContentDescriptor = ammoDescriptor;
            return operationResult;
        }

        public static ImportOperationResult AddFireMode(FireModeRecord firemodeRecord, CustomFireModeDescriptor fireModeDescriptor, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();
            Logger.LogDebug($"Attempting to add firemode");

            if (firemodeRecord is null)
            {
                operationResult.AddError("Firemode record is null.");
                return operationResult;
            }
            else if (firemodeRecord.Id is null || firemodeRecord.Id.Trim() is "")
            {
                operationResult.AddError("Firemode ID is null or empty.");
                return operationResult;
            }
            else if (fireModeDescriptor is null)
            {
                operationResult.AddError("Firemode content descriptor is null.");
                return operationResult;
            }

            Logger.LogDebug($"Valid call to {nameof(AddFireMode)} with ID: " + firemodeRecord.Id);
            var descriptorAssignmentResult = firemodeRecord.SetFireModeDescriptorProperties(fireModeDescriptor, assetFolderPath);
            operationResult.Absorb(descriptorAssignmentResult);

            if (!descriptorAssignmentResult.IsSuccess)
            {
                operationResult.AddError($"Failed to set firemode descriptor properties for firemode with ID: {firemodeRecord.Id}. Firemode won't be added to the game.");
                return operationResult;
            }

            if (QuasimorphHelper.IsGameId(firemodeRecord.Id, Data.Firemodes))
            {
                Data.Firemodes.RemoveRecord(firemodeRecord.Id);
                operationResult.AddWarning($"Firemode with ID: [{firemodeRecord.Id}] was overriden");
            }

            Logger.LogDebug($"Adding firemode with ID: {firemodeRecord.Id} to the game.");
            Data.Descriptors["firemodes"].AddDescriptor(firemodeRecord.Id, firemodeRecord.ContentDescriptor);
            Data.Firemodes.AddRecord(firemodeRecord.Id, firemodeRecord);
            operationResult.ContentList.Add(firemodeRecord.Id);

            return operationResult;
        }

        internal static ImportOperationResult AddConsumable(ConsumableRecord consumable, CustomConsumableDescriptor descriptor, string assetFolderPath)
        {
            // Consumable has UseSound, which is a sound that has to be imported or extracted.
            // Otherwise it has images, and a few simple properties.
            // The process is similar to others above, and it is also considered an Item.
            var operationResult = new ImportOperationResult();

            Logger.LogDebug($"Attempting to add consumable");

            if (consumable is null)
            {
                operationResult.AddError("Firemode record is null.");
                return operationResult;
            }
            else if (consumable.Id is null || consumable.Id.Trim() is "")
            {
                operationResult.AddError("Consumable ID is null or empty.");
                return operationResult;
            }
            else if (descriptor is null)
            {
                operationResult.AddError($"Consumable content descriptor for {consumable.Id} is null.");
                return operationResult;
            }

            var opResult = consumable.SetDescriptorProperties(descriptor, assetFolderPath);
            operationResult.Absorb(opResult);

            var addItemResult = AddItemToGame(consumable);
            operationResult.Absorb(addItemResult);

            return operationResult;
        }

        internal static ImportOperationResult AddTrait(ItemTraitRecord itemTrait)
        {
            var operationResult = new ImportOperationResult(); 
            Logger.LogDebug($"Attempting to add trait");
            if (QuasimorphHelper.IsGameId(itemTrait.Id, Data.ItemTraits))
            {
                Data.ItemTraits.RemoveRecord(itemTrait.Id);
                operationResult.AddWarning($"Trait with ID: [{itemTrait.Id}] was overriden.");
            }

            MGSC.Data.ItemTraits.AddRecord(itemTrait.Id, itemTrait);
            operationResult.ContentList.Add(itemTrait.Id);
            Logger.LogDebug($"Added trait with ID: {itemTrait.Id}.");
            return operationResult;
        }

        public static ImportOperationResult AddExplosion(ExplosionRecord explosionRecord, CustomExplosionDescriptor descriptor, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();

            var descriptorOperation = SetExplosionDescriptorProperties(explosionRecord, descriptor, assetFolderPath);
            operationResult.Absorb(descriptorOperation);
            if (!descriptorOperation.IsSuccess)
            {
                Logger.LogError($"Failed to set explosion descriptor properties for explosion with ID: {explosionRecord.Id}. Explosion won't be added to the game.");
                return operationResult;
            }

            if (QuasimorphHelper.IsGameId(explosionRecord.Id, Data.Explosions))
            {
                Data.Explosions.RemoveRecord(explosionRecord.Id);
                operationResult.AddWarning($"Explosion with ID: [{explosionRecord.Id}] was overriden");
            }

            Logger.LogDebug($"Adding explosion with ID: {explosionRecord.Id} to the game.");
            Data.Descriptors["explosions"].AddDescriptor(explosionRecord.Id, explosionRecord.ContentDescriptor);
            Data.Explosions.AddRecord(explosionRecord.Id, explosionRecord);

            return operationResult;
        }

        private static ImportOperationResult SetExplosionDescriptorProperties(ExplosionRecord explosionRecord, CustomExplosionDescriptor customExplosionDescriptor, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();
            var descriptor = ScriptableObject.CreateInstance<ExplosionDescriptor>();

            Logger.LogDebug($"Setting explosion descriptor properties for explosion with ID: {explosionRecord.Id}");
            var explosionValue = QuasimorphHelper.GetPropertyFromList<ExplosionRecord, ExplosionDescriptor>(customExplosionDescriptor.ExplosionVisualId, "explosion", Data.Explosions);
            if (explosionValue is null)
            {
                operationResult.AddError($"Failed to load explosion icon sprite from path: {customExplosionDescriptor.ExplosionVisualId}");
                return operationResult;
            }
            descriptor.explosion = explosionValue as GameObject;

            var opResult = QuasimorphHelper.ResolveSoundBank<ExplosionRecord, ExplosionDescriptor>(ref descriptor.explosionSoundBank, customExplosionDescriptor.ExplosionSoundIdOrPath, assetFolderPath, Data.Explosions);
            operationResult.Absorb(opResult);
            if (!opResult.IsSuccess)
            {
                Logger.LogError($"Failed to set explosion sound for explosion with ID: {customExplosionDescriptor.ExplosionSoundIdOrPath}.");
                return operationResult;
            }

            descriptor.visualExplosionOffset = new Vector3(customExplosionDescriptor.VisualExplosionOffsetX, customExplosionDescriptor.VisualExplosionOffsetY, customExplosionDescriptor.VisualExplosionOffsetZ);
            descriptor.visualExplosionDelay = customExplosionDescriptor.VisualExplosionDelay;
            descriptor.visualReachCellDuration = customExplosionDescriptor.VisualReachCellDuration;
            descriptor.shakeCameraOnExplosion = customExplosionDescriptor.ShakeCameraOnExplosion;
            descriptor.clearGibsRadiusInPixels = customExplosionDescriptor.ClearGibsRadiusInPixels;

            Logger.LogDebug($"Successfully loaded explosion icon for explosion with ID: {explosionRecord.Id}");
            explosionRecord.ContentDescriptor = descriptor;
            return operationResult;
        }
    }
}