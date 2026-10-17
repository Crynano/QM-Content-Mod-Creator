using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Extensions.Descriptors;
using QM_ImporterAPI.Services.Validation;
using QM_ImporterAPI.Templates.Descriptors;

namespace QM_ImporterAPI.Services.Loaders
{
    /// <summary>
    /// Loader for usable items.
    /// </summary>
    internal class UsableItemLoader : BaseItemLoader<UsableItemRecord, CustomUsableDescriptor>
    {
        protected override string LoaderName => nameof(UsableItemLoader);

        public UsableItemLoader() : base(new UsableItemValidator())
        {
            
        }

        protected override ImportOperationResult Create(UsableItemRecord usableItemRecord, CustomUsableDescriptor descriptor, string assetFolderPath)
        {
            var result = base.Create(usableItemRecord, descriptor, assetFolderPath);

            var descriptorPropertiesResult = usableItemRecord.SetItemContentDescriptorProperties(descriptor, assetFolderPath);
            result.Absorb(descriptorPropertiesResult);

            if (!result.IsSuccess)
            {
                return result;
            }

            var addItemToGame = ItemCreator.AddItemToGame(usableItemRecord);
            result.Absorb(addItemToGame);

            return result;
        }
    }
}
