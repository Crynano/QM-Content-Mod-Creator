using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Services.Loaders
{
    /// <summary>
    /// Loader for item trait records.
    /// </summary>
    public class TraitLoader : BaseItemLoader
    {
        protected override string LoaderName => nameof(TraitLoader);

        public override ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();

            var traitRecords = FilterByType<ItemTraitRecord>(deserializedObjects);

            LogLoadStart(traitRecords.Count());

            foreach (var traitRecord in traitRecords)
            {
                var result = ItemCreator.AddTrait(traitRecord);
                operationResult.Absorb(result);
            }

            return operationResult;
        }
    }
}
