using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Templates.Descriptors;
using System.Collections.Generic;
using System.IO;

namespace QM_ImporterAPI.Services.Loaders
{
    internal class SpriteImageLoader : BaseItemLoader
    {
        protected override string LoaderName => nameof(SpriteImageLoader);

        public override ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();
            var tooltipImages = FilterByType<CustomTooltipImage>(deserializedObjects);

            var gameSprites = ToTooltip(assetFolderPath, tooltipImages);
            foreach (var item in gameSprites)
            {
                Data.TooltipIcons._entries.Add(item);
            }

            return operationResult;
        }

        private List<TooltipIconEntry> ToTooltip(string assetFolderPath, IEnumerable<CustomTooltipImage> tooltipImages)
        {
            var result = new List<TooltipIconEntry>();

            foreach (var image in tooltipImages)
            {
                var sprite = AssetImporter.LoadSpriteWithDefaultScaling(Path.Combine(assetFolderPath, image.SpritePathOrId));
                var entry = new TooltipIconEntry
                {
                    Sprite = sprite,
                    Tag = image.Tag
                };
                result.Add(entry);
            }

            return result;
        }
    }
}