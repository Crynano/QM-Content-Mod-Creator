using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Templates.Descriptors;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Services.Loaders
{
    /// <summary>
    /// Loader for ammunition items.
    /// </summary>
    public class AmmoLoader : BaseItemLoader
    {
        protected override string LoaderName => nameof(AmmoLoader);

        public override ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();

            var ammoRecords = FilterByType<AmmoRecord>(deserializedObjects);
            var ammoDescriptors = FilterByType<CustomAmmoDescriptor>(deserializedObjects);

            LogLoadStart(ammoRecords.Count(), ammoDescriptors.Count());

            foreach (var descriptor in ammoDescriptors)
            {
                var ammoRecord = ammoRecords.FirstOrDefault(x => x.Id.Equals(descriptor.ItemId));
                if (ammoRecord != null)
                {
                    Logger.LogDebug($"Trying to add ammo '{ammoRecord.Id}' (with descriptor) to the game!");
                    var opResult = ItemCreator.AddAmmo(ammoRecord, descriptor, assetFolderPath);
                    operationResult.Absorb(opResult);
                }
                else
                {
                    operationResult.AddWarning($"Could not find an ammo record with id '{descriptor.ItemId}' for the ammo descriptor. Skipping this ammo.");
                }
            }

            return operationResult;
        }
    }
}
