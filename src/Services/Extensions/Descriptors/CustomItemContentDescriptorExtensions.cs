using MGSC;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Templates.Descriptors;

namespace QM_ImporterAPI.Services.Extensions.Descriptors
{
    internal static class CustomItemContentDescriptorExtensions
    {
        internal static void LoadSprites<TDesciptor>(this TDesciptor descriptor, CustomItemContentDescriptor customItemDescriptor, string assetFolderPath) where TDesciptor : ItemContentDescriptor
        {
            var imageProps = customItemDescriptor.ImageProperties;

            descriptor._icon.LoadSprite(assetFolderPath, imageProps.IconSpriteIdOrPath, "Icon", AssetImporter.LoadNewSprite);
            descriptor._smallIcon.LoadSprite(assetFolderPath, imageProps.SmallIconSpriteIdOrPath, "SmallIcon", AssetImporter.LoadCenteredSprite);
            descriptor._shadow.LoadSprite(assetFolderPath, imageProps.ShadowOnFloorSpriteIdOrPath, "Shadow", AssetImporter.LoadCenteredSprite);
        }

        internal static ItemContentDescriptor ToItemContentDescriptor(this CustomItemContentDescriptor customItemDescriptor, ItemContentDescriptor descriptor, string assetFolderPath)
        {
            var imageProps = customItemDescriptor.ImageProperties;

            descriptor._icon.LoadSprite(assetFolderPath, imageProps.IconSpriteIdOrPath, "Icon", AssetImporter.LoadNewSprite);
            descriptor._smallIcon.LoadSprite(assetFolderPath, imageProps.SmallIconSpriteIdOrPath, "SmallIcon", AssetImporter.LoadCenteredSprite);
            descriptor._shadow.LoadSprite(assetFolderPath, imageProps.ShadowOnFloorSpriteIdOrPath, "Shadow", AssetImporter.LoadCenteredSprite);
            return descriptor;
        }
    }
}