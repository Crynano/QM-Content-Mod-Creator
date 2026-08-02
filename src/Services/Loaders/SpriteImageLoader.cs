using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Templates.Descriptors;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
                var existingEntry = Data.TooltipIcons._entries.Find(x => x.Tag.Equals(item.Tag));
                var index = Data.TooltipIcons._entries.IndexOf(existingEntry);
                if (index < 0)
                {
                    Logger.LogDebug($"Adding new tooltip icon: '{item.Tag}'");
                    Data.TooltipIcons._entries.Add(item);
                }
                else
                {
                    existingEntry.Sprite = item.Sprite;
                    existingEntry.SpriteName = item.SpriteName;
                    operationResult.AddWarning($"Replacing tooltip icon: '{item.Tag}' with new sprite at Index: {index}");
                }
            }

            return operationResult;
        }

        private List<TooltipIconEntry> ToTooltip(string assetFolderPath, IEnumerable<CustomTooltipImage> tooltipImages)
        {
            var result = new List<TooltipIconEntry>();

            foreach (var image in tooltipImages)
            {
                var sprite = AssetImporter.LoadSpriteCustom(Path.Combine(assetFolderPath, image.SpritePathOrId), new Vector2(0.5f, 0.5f), 1);
                var entry = new TooltipIconEntry
                {
                    Sprite = sprite,
                    SpriteName = image.SpritePathOrId,
                    Tag = image.Tag
                };
                result.Add(entry);
            }

            return result;
        }
    }
}