using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Extensions;
using QM_ImporterAPI.Services.Extensions.Descriptors;
using QM_ImporterAPI.Services.Helpers;
using QM_ImporterAPI.Services.Images;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Services.Validation;
using QM_ImporterAPI.Templates.Descriptors;
using System.Collections.Generic;
using UnityEngine;

namespace QM_ImporterAPI.Services.Loaders
{
    internal class HelmetLoader : BaseItemLoader<HelmetRecord, CustomHelmetDescriptor>
    {
        protected override string LoaderName => nameof(HelmetLoader);

        public HelmetLoader() : base(new ResistValidator<HelmetRecord, CustomHelmetDescriptor>())
        {
            
        }

        protected override ImportOperationResult Create(HelmetRecord record, CustomHelmetDescriptor descriptor, string assetFolderPath)
        {
            var result = base.Create(record, descriptor, assetFolderPath);

            var descriptorPropertiesResult = SetHelmetDescriptorProperties(record, descriptor, assetFolderPath, out var helmetPrefab);
            result.Absorb(descriptorPropertiesResult);

            if (!result.IsSuccess)
            {
                return result;
            }

            var addItemToGame = ItemCreator.AddItemToGame(record);
            result.Absorb(addItemToGame);

            if (addItemToGame.IsSuccess && helmetPrefab != null)
            {
                RegisterHelmetPrefab(descriptor, helmetPrefab, result);
            }

            return result;
        }

        internal static ImportOperationResult SetHelmetDescriptorProperties<TRecord>(TRecord record, CustomHelmetDescriptor customBaseDescriptor, string assetFolderPath) where TRecord : ItemRecord
        {
            return SetHelmetDescriptorProperties(record, customBaseDescriptor, assetFolderPath, out _);
        }

        private static ImportOperationResult SetHelmetDescriptorProperties<TRecord>(TRecord record, CustomHelmetDescriptor customBaseDescriptor, string assetFolderPath, out GameObject helmetPrefab) where TRecord : ItemRecord
        {
            var result = new ImportOperationResult();
            helmetPrefab = null;
            var baseDescriptor = ScriptableObject.CreateInstance<HelmetDescriptor>();

            if (customBaseDescriptor == null)
            {
                return result.AddWarning($"{nameof(CustomHelmetDescriptor)} for {record.Id} is null.");
            }

            if (customBaseDescriptor.Part == null)
            {
                return result.AddError($"{nameof(CustomHelmetDescriptor)} for {record.Id} must contain one armor part.");
            }

            if (string.IsNullOrWhiteSpace(customBaseDescriptor.PrefabIdOrPath))
            {
                return result.AddWarning($"The armor part for {record.Id} must specify a {nameof(CustomHelmetDescriptor.PrefabIdOrPath)} when using CustomHelmetDescriptor.");
            }

            var parts = customBaseDescriptor.Part.ToGameList();
            var part = parts[0];
            var texResult = TextureImporter.ImportFromFile(assetFolderPath, customBaseDescriptor.Part.TextureIdOrPath);
            result.Absorb(texResult);

            part.Texture = texResult.Result;
            parts[0] = part;

            if (QuasimorphHelper.IsGameId(customBaseDescriptor.PrefabIdOrPath))
            {
                // Here we must try to load the prefab from the game assets instead of the external file system.
                var prefabResult = QuasimorphHelper.GetHelmetPrefab(customBaseDescriptor.PrefabIdOrPath);
                result.Absorb(prefabResult);

                helmetPrefab = prefabResult.Result;
            }

            if (!QuasimorphHelper.IsGameId(customBaseDescriptor.PrefabIdOrPath) && helmetPrefab == null)
            {
                var prefabResult = PrefabFactory.LoadPrefab(customBaseDescriptor.PrefabIdOrPath, assetFolderPath);
                result.Absorb(prefabResult);
                if (!prefabResult.IsSuccess)
                {
                    return result;
                }

                helmetPrefab = prefabResult.Result;
                helmetPrefab.name = customBaseDescriptor.Part.ArmorPart;

                var itemBone = helmetPrefab.GetComponent<ItemBone>() ?? helmetPrefab.AddComponent<ItemBone>();

                itemBone.TargetBoneId = "Head";
                itemBone.Scale = new Vector3(.12f, .12f, .12f);
            }

            baseDescriptor.LoadSprites(customBaseDescriptor, assetFolderPath);
            baseDescriptor._parts = parts;

            record.ContentDescriptor = baseDescriptor;
            return result;
        }

        private static void RegisterHelmetPrefab(CustomHelmetDescriptor descriptor, GameObject prefab, ImportOperationResult operationResult)
        {
            var part = descriptor.Part;
            var eligibleActorCount = 0;

            foreach (ActorRecord actorRecord in Data.Actors.Records)
            {
                if (!(actorRecord.ContentDescriptor is ActorDescriptor actorDescriptor) ||
                    actorDescriptor.ArmorArchTypes == null ||
                    !actorDescriptor.ArmorArchTypes.Exists(arch => arch.ArmorType == "ArmorHeavy"))
                {
                    continue;
                }

                eligibleActorCount++;
                var bones = actorDescriptor.LoadedMesh?.BonesIds;
                if (bones == null || !bones.Contains("Head"))
                {
                    operationResult.AddWarning($"Helmet prefab '{prefab.name}' was not offered to actor '{actorRecord.Id}': it has no 'Head' bone.");
                    continue;
                }

                var armorArchTypeIndex = actorDescriptor.ArmorArchTypes.FindIndex(arch => arch.ArmorType == part.ArmorType);
                var armorArchType = armorArchTypeIndex < 0
                    ? new ArmorArchType
                    {
                        ArmorType = part.ArmorType,
                        Prefabs = new List<GameObject>()
                    }
                    : actorDescriptor.ArmorArchTypes[armorArchTypeIndex];

                if (armorArchType.Prefabs == null)
                {
                    armorArchType.Prefabs = new List<GameObject>();
                }

                var prefabIndex = armorArchType.Prefabs.FindIndex(existingPrefab => existingPrefab != null && existingPrefab.name == prefab.name);
                if (prefabIndex < 0)
                {
                    armorArchType.Prefabs.Add(prefab);
                }
                else
                {
                    operationResult.AddWarning($"A helmet prefab named '{prefab.name}' already exists for actor '{actorRecord.Id}' and was replaced. Each helmet needs a unique ArmorPart.");
                    armorArchType.Prefabs[prefabIndex] = prefab;
                }

                if (armorArchTypeIndex < 0)
                {
                    actorDescriptor.ArmorArchTypes.Add(armorArchType);
                }
                else
                {
                    actorDescriptor.ArmorArchTypes[armorArchTypeIndex] = armorArchType;
                }
            }

            if (eligibleActorCount == 0)
            {
                operationResult.AddWarning($"Helmet prefab '{prefab.name}' was not offered to any actors because none support ArmorHeavy.");
            }
        }
    }
}
