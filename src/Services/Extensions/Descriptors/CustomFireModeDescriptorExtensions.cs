using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Templates.Descriptors;
using UnityEngine;

namespace QM_ImporterAPI.Services.Extensions.Descriptors
{
    internal static class CustomFireModeDescriptorExtensions
    {
        internal static ImportOperationResult SetFireModeDescriptorProperties(this FireModeRecord ammoRecord, CustomFireModeDescriptor customAmmoDescriptor, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();
            var descriptor = ScriptableObject.CreateInstance<FireModeDescriptor>();
            Logger.LogDebug($"Setting firemode descriptor properties for firemode with ID: {ammoRecord.Id}");

            descriptor.Icon.LoadSprite(assetFolderPath, customAmmoDescriptor.SpriteIdOrPath, "Icon", AssetImporter.LoadNewSprite);
            if (descriptor.Icon is null)
            {
                Logger.LogError($"Failed to add icon to {ammoRecord.Id}.");
                operationResult.AddError($"Failed to load firemode icon sprite from path: {customAmmoDescriptor.SpriteIdOrPath}");
                return operationResult;
            }
            Logger.LogDebug($"Successfully loaded firemode icon for firemode with ID: {ammoRecord.Id}");
            ammoRecord.ContentDescriptor = descriptor;
            return operationResult;
        }
    }
}
