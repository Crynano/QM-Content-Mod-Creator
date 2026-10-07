using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Templates.Descriptors;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Services.Loaders
{
    /// <summary>
    /// Loader for trash item records.
    /// </summary>
    internal class TrashLoader : BaseItemLoader
    {
        protected override string LoaderName => nameof(TrashLoader);

        public override ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();

            var trashRecords = FilterByType<TrashRecord>(deserializedObjects);
            var trashDescriptors = FilterByType<CustomTrashDescriptor>(deserializedObjects);

            LogLoadStart(trashRecords.Count(), trashDescriptors.Count());

            foreach (var trash in trashRecords)
            {
                var descriptor = trashDescriptors.FirstOrDefault(x => x.ItemId.Equals(trash.Id));
                var opResult = ItemCreator.AddTrash(trash, descriptor, assetFolderPath);
                operationResult.Absorb(opResult);
            }

            return operationResult;
        }
    }
}
