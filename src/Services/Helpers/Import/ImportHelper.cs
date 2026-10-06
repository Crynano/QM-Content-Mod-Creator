using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Templates.Descriptors;

namespace QM_ImporterAPI.Services.Helpers.Import
{
    internal static class ImportHelper
    {
        public static ImportOperationResult PerformImportValidation<TRecord, TDescriptor>(TRecord record, TDescriptor descriptor) 
            where TRecord : ConfigTableRecord where TDescriptor : CustomBaseDescriptor
        {
            var operationResult = new ImportOperationResult();

            if (record is null)
            {
                operationResult.AddError($"Record of type {typeof(TRecord)} is null.");
            }
            else if (string.IsNullOrEmpty(record.Id))
            {
                operationResult.AddError($"Record ID for type {typeof(TRecord)} is null or empty.");
            }
            else if (descriptor is null)
            {
                operationResult.AddError($"Content descriptor for record ID {record.Id} is null.");
            }

            return operationResult;
        }
    }
}
