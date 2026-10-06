using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Extensions.Descriptors;
using QM_ImporterAPI.Services.Helpers.Import;
using QM_ImporterAPI.Services.Validation;
using QM_ImporterAPI.Templates.Descriptors;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Services.Loaders
{
    internal class AugmentationLoader : BaseItemLoader<AugmentationRecord, CustomAugmentationDescriptor>
    {
        protected override string LoaderName => nameof(AugmentationLoader);
        private static AugmentationValidator AugmentationValidator { get; set; }
        public AugmentationLoader()
        {
            AugmentationValidator = new AugmentationValidator();
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

            //// Load augmentation records without descriptors (replacements)
            //var augmentationRecordsWithoutDescriptor = augmentationRecords
            //    .Where(ar => !augmentationDescriptors.Any(d => d.ItemId.Equals(ar.Id)))
            //    .ToList();

            //foreach (var augmentationRecord in augmentationRecordsWithoutDescriptor)
            //{
            //    Logger.LogDebug($"Trying to add augmentation '{augmentationRecord.Id}' (without descriptor) to the game!");
            //    var opResult = ItemCreator.ReplaceAugmentation(augmentationRecord, assetFolderPath);
            //    operationResult.Absorb(opResult);
            //}

            return operationResult;
        }

        protected override ImportOperationResult Create(AugmentationRecord augmentationRecord, CustomAugmentationDescriptor descriptor, string assetFolderPath)
        {
            Logger.LogDebug($"{nameof(Create)} Creating augmentation '{augmentationRecord.Id}' with descriptor '{descriptor.ItemId}'");
            var result = new ImportOperationResult();

            var validationResult = ImportHelper.PerformImportValidation(augmentationRecord, descriptor);
            result.Absorb(validationResult);

            var augmentationValidation = AugmentationValidator.Validate(augmentationRecord, descriptor);
            result.Absorb(augmentationValidation);

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
