using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Templates.Descriptors;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Services.Loaders
{
    /// <summary>
    /// Loader for mercenary class items. Handles mercenary classes with descriptors and mercenary classes without descriptors.
    /// </summary>
    public class MercenaryClassLoader : BaseItemLoader
    {
        protected override string LoaderName => nameof(MercenaryClassLoader);

        public override ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();

            var mercenaryClassRecords = FilterByType<MercenaryClassRecord>(deserializedObjects);
            var mercenaryClassDescriptors = FilterByType<CustomMercenaryClassDescriptor>(deserializedObjects);

            LogLoadStart(mercenaryClassRecords.Count(), mercenaryClassDescriptors.Count());

            // Load mercenary classes with descriptors
            foreach (var descriptor in mercenaryClassDescriptors)
            {
                var mercenaryClassRecord = mercenaryClassRecords.FirstOrDefault(x => x.Id.Equals(descriptor.ItemId));
                if (mercenaryClassRecord != null)
                {
                    Logger.LogDebug($"Trying to add mercenary class '{mercenaryClassRecord.Id}' (with descriptor) to the game!");
                    var opResult = ItemCreator.CreateMercenaryClass(mercenaryClassRecord, descriptor, assetFolderPath);
                    operationResult.Absorb(opResult);
                }
                else
                {
                    operationResult.AddWarning($"Could not find a mercenary class record with id '{descriptor.ItemId}' for the mercenary class descriptor. Skipping this mercenary class.");
                }
            }

            var mercenaryClassRecordsWithoutDescriptor = mercenaryClassRecords
                .Where(mcr => !mercenaryClassDescriptors.Any(d => d.ItemId.Equals(mcr.Id)))
                .ToList();

            foreach (var mercenaryClassRecord in mercenaryClassRecordsWithoutDescriptor)
            {
                Logger.LogDebug($"Trying to add mercenary class '{mercenaryClassRecord.Id}' (without descriptor) to the game!");
                var opResult = ItemCreator.ReplaceMercenaryClass(mercenaryClassRecord, assetFolderPath);
                operationResult.Absorb(opResult);
            }

            return operationResult;
        }
    }
}
