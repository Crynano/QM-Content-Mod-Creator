using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Templates.Descriptors;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Services.Loaders
{
    /// <summary>
    /// Loader for datadisk items.
    /// </summary>
    public class DatadiskLoader : BaseItemLoader
    {
        protected override string LoaderName => nameof(DatadiskLoader);

        public override ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();

            var datadiskRecords = FilterByType<DatadiskRecord>(deserializedObjects);
            var datadiskDescriptors = FilterByType<CustomDatadiskDescriptor>(deserializedObjects);

            LogLoadStart(datadiskRecords.Count(), datadiskDescriptors.Count());

            foreach (var singleDataDisk in datadiskRecords)
            {
                var descriptor = datadiskDescriptors.FirstOrDefault(x => x.ItemId.Equals(singleDataDisk.Id));
                var opResult = ItemCreator.AddDatadiskItems(singleDataDisk, descriptor, assetFolderPath);
                operationResult.Absorb(opResult);
            }

            return operationResult;
        }
    }
}
