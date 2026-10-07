using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Validation.Base;
using QM_ImporterAPI.Templates.Descriptors;

namespace QM_ImporterAPI.Services.Validation
{
    internal class ImplantValidator : ItemRecordValidator<ImplantRecord, CustomImplantDescriptor>
    {
        public override ImportOperationResult Validate(ImplantRecord record, CustomImplantDescriptor descriptor)
        {
            var result = base.Validate(record, descriptor);

            // Add implant validations if needed.

            return result;
        }
    }
}
