using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Helpers;
using QM_ImporterAPI.Templates.Descriptors;
using UnityEngine;

namespace QM_ImporterAPI.Services.Extensions.Descriptors
{
    internal static class CustomConsumableDescriptorExtensions
    {
        internal static ImportOperationResult SetDescriptorProperties(this ConsumableRecord record, CustomConsumableDescriptor customDescriptor, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();

            if (customDescriptor == null)
            {
                operationResult.AddError("CustomConsumableDescriptor is null.");
                return operationResult;
            }

            var descriptor = ScriptableObject.CreateInstance<ConsumableDescriptor>();
            descriptor.LoadSprites(customDescriptor, assetFolderPath);

            var useSoundResult = QuasimorphHelper.GetAudioFromConsumableOrPath(customDescriptor.UseSoundPathOrId, assetFolderPath);
            if (useSoundResult.IsSuccess)
            {
                descriptor._useSound = useSoundResult.Result;
            }
            operationResult.Absorb(useSoundResult);

            record.ContentDescriptor = descriptor;
            return operationResult;
        }
    }
}
