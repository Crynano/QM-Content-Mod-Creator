using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Services.Loaders
{
    /// <summary>
    /// Loader for crafting and transformation recipes.
    /// </summary>
    public class CraftingLoader : BaseItemLoader
    {
        protected override string LoaderName => nameof(CraftingLoader);

        public override ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();

            var transformationRecords = FilterByType<ItemTransformationRecord>(deserializedObjects);
            var craftingRecords = FilterByType<ItemProduceReceipt>(deserializedObjects);

            Logger.LogDebug($"{LoaderName}: Found {transformationRecords.Count()} transformation records and {craftingRecords.Count()} crafting records.");

            var itemTransformResult = ItemCreator.AddItemTransformation(transformationRecords);
            operationResult.Absorb(itemTransformResult);

            var craftRecipesResult = ItemCreator.AddItemCraftRecipe(craftingRecords);
            operationResult.Absorb(craftRecipesResult);

            return operationResult;
        }
    }
}
