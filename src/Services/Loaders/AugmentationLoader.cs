using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Extensions.Descriptors;
using QM_ImporterAPI.Services.Validation;
using QM_ImporterAPI.Templates.Descriptors;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Services.Loaders
{
    internal class AugmentationLoader : BaseItemLoader<AugmentationRecord, CustomAugmentationDescriptor>
    {
        protected override string LoaderName => nameof(AugmentationLoader);
        public AugmentationLoader() : base(new AugmentationValidator())
        {
            
        }

        public override ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();

            var augmentationRecords = FilterByType<AugmentationRecord>(deserializedObjects);
            var augmentationDescriptors = FilterByType<CustomAugmentationDescriptor>(deserializedObjects);

            LogLoadStart(augmentationRecords.Count(), augmentationDescriptors.Count());

            // Load augmentations with descriptors
            foreach (var descriptor in augmentationDescriptors)
            {
                var augmentationRecord = augmentationRecords.FirstOrDefault(x => x.Id.Equals(descriptor.ItemId));
                if (augmentationRecord != null)
                {
                    Logger.LogDebug($"Trying to add augmentation '{augmentationRecord.Id}' (with descriptor) to the game!");
                    var opResult = Create(augmentationRecord, descriptor, assetFolderPath);
                    operationResult.Absorb(opResult);
                }
                else
                {
                    operationResult.AddWarning($"Could not find an augmentation record with id '{descriptor.ItemId}' for the augmentation descriptor. Skipping this augmentation.");
                }
            }

            return operationResult;
        }

        protected override ImportOperationResult Create(AugmentationRecord augmentationRecord, CustomAugmentationDescriptor descriptor, string assetFolderPath)
        {
            var result = base.Create(augmentationRecord, descriptor, assetFolderPath);

            var descriptorPropertiesResult = augmentationRecord.SetItemContentDescriptorProperties(descriptor, assetFolderPath);
            result.Absorb(descriptorPropertiesResult);

            if (!result.IsSuccess)
            {
                return result;
            }

            var addItemToGame = ItemCreator.AddAugmentToGame(augmentationRecord);
            result.Absorb(addItemToGame);

            return result;
        }
    }
}
