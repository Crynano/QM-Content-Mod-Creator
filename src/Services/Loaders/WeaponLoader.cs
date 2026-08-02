using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Templates.Descriptors;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Services.Loaders
{
    /// <summary>
    /// Loader for weapon items. Handles weapons with descriptors and weapons without descriptors.
    /// </summary>
    public class WeaponLoader : BaseItemLoader
    {
        protected override string LoaderName => nameof(WeaponLoader);

        public override ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();

            var weaponRecords = FilterByType<WeaponRecord>(deserializedObjects);
            var weaponDescriptors = FilterByType<CustomWeaponDescriptor>(deserializedObjects);

            LogLoadStart(weaponRecords.Count(), weaponDescriptors.Count());

            // Load weapons with descriptors
            foreach (var descriptor in weaponDescriptors)
            {
                var weaponRecord = weaponRecords.FirstOrDefault(x => x.Id.Equals(descriptor.ItemId));
                if (weaponRecord != null)
                {
                    Logger.LogDebug($"Trying to add weapon '{weaponRecord.Id}' (with descriptor) to the game!");
                    var opResult = ItemCreator.CreateWeapon(weaponRecord, descriptor, assetFolderPath);
                    operationResult.Absorb(opResult);
                }
                else
                {
                    operationResult.AddWarning($"Could not find a weapon record with id '{descriptor.ItemId}' for the weapon descriptor. Skipping this weapon.");
                }
            }

            // Load weapon records without descriptors (replacements)
            var weaponRecordsWithoutDescriptor = weaponRecords
                .Where(wr => !weaponDescriptors.Any(d => d.ItemId.Equals(wr.Id)))
                .ToList();

            foreach (var weaponRecord in weaponRecordsWithoutDescriptor)
            {
                Logger.LogDebug($"Trying to add weapon '{weaponRecord.Id}' (without descriptor) to the game!");
                var opResult = ItemCreator.ReplaceWeapon(weaponRecord, assetFolderPath);
                operationResult.Absorb(opResult);
            }

            return operationResult;
        }
    }
}
