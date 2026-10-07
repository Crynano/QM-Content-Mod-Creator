using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Extensions.Descriptors;
using QM_ImporterAPI.Services.Validation;
using QM_ImporterAPI.Templates.Descriptors;

namespace QM_ImporterAPI.Services.Loaders
{
    internal class ImplantLoader : BaseItemLoader<ImplantRecord, CustomImplantDescriptor>
    {
        protected override string LoaderName => nameof(ImplantLoader);

        public ImplantLoader() : base(new ImplantValidator())
        {
            
        }

        protected override ImportOperationResult Create(ImplantRecord record, CustomImplantDescriptor descriptor, string assetFolderPath)
        {
            var result = base.Create(record, descriptor, assetFolderPath);

            var descriptorPropertiesResult = record.SetItemContentDescriptorProperties(descriptor, assetFolderPath);
            result.Absorb(descriptorPropertiesResult);

            if (!result.IsSuccess)
            {
                return result;
            }

            var addItemToGame = ItemCreator.AddItemToGame(record);
            result.Absorb(addItemToGame);

            return result;
        }
    }
}
