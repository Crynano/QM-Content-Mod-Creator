using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Templates.Descriptors;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Services.Loaders
{
    /// <summary>
    /// Loader for explosion items.
    /// </summary>
    public class ExplosionLoader : BaseItemLoader
    {
        protected override string LoaderName => nameof(ExplosionLoader);

        public override ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();

            var explosionRecords = FilterByType<ExplosionRecord>(deserializedObjects);
            var explosionDescriptors = FilterByType<CustomExplosionDescriptor>(deserializedObjects);

            LogLoadStart(explosionRecords.Count(), explosionDescriptors.Count());

            foreach (var descriptor in explosionDescriptors)
            {
                var explosionRecord = explosionRecords.FirstOrDefault(x => x.Id.Equals(descriptor.ItemId));
                if (explosionRecord != null)
                {
                    Logger.LogDebug($"Trying to add {nameof(ExplosionRecord)} '{explosionRecord.Id}' (with descriptor) to the game!");
                    var opResult = ItemCreator.AddExplosion(explosionRecord, descriptor, assetFolderPath);
                    operationResult.Absorb(opResult);
                }
                else
                {
                    operationResult.AddWarning($"Could not find an explosion record with id '{descriptor.ItemId}' for the explosion descriptor. Skipping this explosion.");
                }
            }

            return operationResult;
        }
    }
}
