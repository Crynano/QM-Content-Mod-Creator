using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Validation.Base;
using QM_ImporterAPI.Templates.Descriptors;

namespace QM_ImporterAPI.Services.Validation
{
    internal class UsableItemValidator : ItemRecordValidator<UsableItemRecord, CustomUsableDescriptor>
    {
        public override ImportOperationResult Validate(UsableItemRecord record, CustomUsableDescriptor descriptor)
        {
            var result = base.Validate(record, descriptor);

            // Add usable item validations if needed.

            return result;
        }
    }
}
