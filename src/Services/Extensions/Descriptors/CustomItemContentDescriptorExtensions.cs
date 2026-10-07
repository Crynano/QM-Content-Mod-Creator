using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Helpers;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Templates.Descriptors;
using UnityEngine;

namespace QM_ImporterAPI.Services.Extensions.Descriptors
{
    internal static class CustomItemContentDescriptorExtensions
    {
        internal static void LoadSprites<TDesciptor>(this TDesciptor descriptor, CustomItemContentDescriptor customItemDescriptor, string assetFolderPath) where TDesciptor : ItemContentDescriptor
        {
            var imageProps = customItemDescriptor.ImageProperties;

            descriptor._icon = QuasimorphHelper.LoadSpriteFromItem<TDesciptor>(assetFolderPath, imageProps.IconSpriteIdOrPath, nameof(ItemContentDescriptor.Icon), AssetImporter.LoadNewSprite);
            descriptor._smallIcon = QuasimorphHelper.LoadSpriteFromItem<TDesciptor>(assetFolderPath, imageProps.SmallIconSpriteIdOrPath, nameof(ItemContentDescriptor.SmallIcon), AssetImporter.LoadOffsetSprite);
            descriptor._shadow = QuasimorphHelper.LoadSpriteFromItem<TDesciptor>(assetFolderPath, imageProps.ShadowOnFloorSpriteIdOrPath, nameof(ItemContentDescriptor.ShadowOnFloor), AssetImporter.LoadOffsetSprite);
        }

        internal static ItemContentDescriptor ToItemContentDescriptor(this CustomItemContentDescriptor customItemDescriptor, ItemContentDescriptor descriptor, string assetFolderPath)
        {
            var imageProps = customItemDescriptor.ImageProperties;

            descriptor._icon = QuasimorphHelper.LoadSpriteFromWeapon(assetFolderPath, imageProps.IconSpriteIdOrPath, nameof(ItemContentDescriptor.Icon), AssetImporter.LoadNewSprite);
            descriptor._smallIcon = QuasimorphHelper.LoadSpriteFromWeapon(assetFolderPath, imageProps.SmallIconSpriteIdOrPath, nameof(ItemContentDescriptor.SmallIcon), AssetImporter.LoadOffsetSprite);
            descriptor._shadow = QuasimorphHelper.LoadSpriteFromWeapon(assetFolderPath, imageProps.ShadowOnFloorSpriteIdOrPath, nameof(ItemContentDescriptor.ShadowOnFloor), AssetImporter.LoadOffsetSprite);
            return descriptor;
        }

        internal static ImportOperationResult SetItemContentDescriptorProperties<TRecord>(this TRecord record, CustomItemContentDescriptor customBaseDescriptor, string assetFolderPath) where TRecord : ItemRecord
        {
            var operationResult = new ImportOperationResult();
            var baseDescriptor = ScriptableObject.CreateInstance<ItemContentDescriptor>();

            if (customBaseDescriptor == null)
            {
                operationResult.AddWarning($"{nameof(CustomItemContentDescriptor)} for {record.Id} is null.");
                return operationResult;
            }

            baseDescriptor.LoadSprites(customBaseDescriptor, assetFolderPath);
            record.ContentDescriptor = baseDescriptor;
            return operationResult;
        }

        internal static ImportOperationResult SetImplantDescriptorProperties(this ImplantRecord record, CustomImplantDescriptor customImplantDescriptor, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();
            var implantDescriptor = ScriptableObject.CreateInstance<ImplantDescriptor>();
            if (customImplantDescriptor == null)
            {
                operationResult.AddWarning($"{nameof(CustomImplantDescriptor)} for {record.Id} is null.");
                return operationResult;
            }
            implantDescriptor.LoadSprites(customImplantDescriptor, assetFolderPath);
            // TODO: Implement sound loading for implants
            //implantDescriptor._useSound = QuasimorphHelper.LoadAudioClipFromExternalFile(assetFolderPath, customImplantDescriptor.UseSoundPath, nameof(ImplantDescriptor.UseSound), AssetImporter);
            record.ContentDescriptor = implantDescriptor;
            return operationResult;
        }
    }
}