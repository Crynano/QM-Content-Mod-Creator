using MGSC;
using QM_ImporterAPI.Services.Helpers;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Templates.Descriptors;

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
    }
}