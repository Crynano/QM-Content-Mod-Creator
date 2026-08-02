using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Templates.Descriptors;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Services.Loaders
{
    /// <summary>
    /// Loader for consumable items.
    /// </summary>
    public class ConsumableLoader : BaseItemLoader
    {
        protected override string LoaderName => nameof(ConsumableLoader);

        public override ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();

            var consumableRecords = FilterByType<ConsumableRecord>(deserializedObjects);
            var consumableDescriptors = FilterByType<CustomConsumableDescriptor>(deserializedObjects);

            LogLoadStart(consumableRecords.Count(), consumableDescriptors.Count());

            foreach (var consumable in consumableRecords)
            {
                var descriptor = consumableDescriptors.FirstOrDefault(x => x.ItemId.Equals(consumable.Id));
                var opResult = ItemCreator.AddConsumable(consumable, descriptor, assetFolderPath);
                operationResult.Absorb(opResult);
            }

            return operationResult;
        }
    }
}
