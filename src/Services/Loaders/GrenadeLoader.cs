using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Templates.Descriptors;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Services.Loaders
{
    /// <summary>
    /// Loader for grenade items.
    /// </summary>
    public class GrenadeLoader : BaseItemLoader
    {
        protected override string LoaderName => nameof(GrenadeLoader);

        public override ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();

            var grenadeRecords = FilterByType<GrenadeRecord>(deserializedObjects);
            var grenadeDescriptors = FilterByType<CustomGrenadeDescriptor>(deserializedObjects);

            LogLoadStart(grenadeRecords.Count(), grenadeDescriptors.Count());

            foreach (var grenade in grenadeRecords)
            {
                var descriptor = grenadeDescriptors.FirstOrDefault(x => x.ItemId.Equals(grenade.Id));
                var opResult = ItemCreator.AddGrenade(grenade, descriptor, assetFolderPath);
                operationResult.Absorb(opResult);
            }

            return operationResult;
        }
    }
}
