using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Templates;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Services.Loaders
{
    /// <summary>
    /// Loader for faction reward templates.
    /// </summary>
    public class FactionRewardsLoader : BaseItemLoader
    {
        protected override string LoaderName => nameof(FactionRewardsLoader);

        public override ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();

            var factionRecords = FilterByType<FactionTemplate>(deserializedObjects);

            LogLoadStart(factionRecords.Count());

            foreach (var faction in factionRecords)
            {
                var result = ItemCreator.AddFactionRewards(faction);
                operationResult.Absorb(result);
            }

            return operationResult;
        }
    }
}
