using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Extensions.Descriptors;
using QM_ImporterAPI.Templates.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QM_ImporterAPI.Services.Extensions.Records
{
    internal static class TrashRecordExtensions
    {
        internal static ImportOperationResult SetDescriptorProperties(this TrashRecord trash, CustomTrashDescriptor customTrashDescriptor, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();
            var trashDescriptor = ScriptableObject.CreateInstance<ItemContentDescriptor>();

            if (customTrashDescriptor == null)
            {
                operationResult.AddError("CustomTrashDescriptor is null.");
                return operationResult;
            }

            trashDescriptor.LoadSprites(customTrashDescriptor, assetFolderPath);
            trash.ContentDescriptor = trashDescriptor;
            return operationResult;
        }
    }
}
