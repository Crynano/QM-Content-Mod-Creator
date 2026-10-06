using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Templates.Descriptors;

namespace QM_ImporterAPI.Services.Validation.Base
{
    internal class ConfigTableRecordValidator<TRecord, TDescriptor> : ValidatorBase<TRecord, TDescriptor>
        where TRecord : ConfigTableRecord
        where TDescriptor : CustomBaseDescriptor
    {
        public override ImportOperationResult Validate(TRecord record, TDescriptor descriptor)
        {
            var result = new ImportOperationResult();

            if (record == null)
            {
                result.AddError("ConfigTableRecord is null.");
                return result;
            }

            if (string.IsNullOrEmpty(record.Id))
            {
                result.AddError("ConfigTableRecord has an invalid Id.");
            }

            return result;
        }
    }
}
