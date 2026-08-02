using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Templates.Descriptors;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Services.Loaders
{
    /// <summary>
    /// Loader for fire mode items.
    /// </summary>
    public class FireModeLoader : BaseItemLoader
    {
        protected override string LoaderName => nameof(FireModeLoader);

        public override ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();

            var firemodeRecords = FilterByType<FireModeRecord>(deserializedObjects);
            var firemodeDescriptors = FilterByType<CustomFireModeDescriptor>(deserializedObjects);

            LogLoadStart(firemodeRecords.Count(), firemodeDescriptors.Count());

            foreach (var descriptor in firemodeDescriptors)
            {
                var firemodeRecord = firemodeRecords.FirstOrDefault(x => x.Id.Equals(descriptor.ItemId));
                if (firemodeRecord != null)
                {
                    Logger.LogDebug($"Trying to add firemode '{firemodeRecord.Id}' (with descriptor) to the game!");
                    var opResult = ItemCreator.AddFireMode(firemodeRecord, descriptor, assetFolderPath);
                    operationResult.Absorb(opResult);
                }
                else
                {
                    operationResult.AddWarning($"Could not find a firemode record with id '{descriptor.ItemId}' for the firemode descriptor. Skipping this firemode.");
                }
            }

            return operationResult;
        }
    }
}
