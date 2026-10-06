using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Templates.Descriptors;

namespace QM_ImporterAPI.Services.Validation.Base
{
    // This class is the base validator functions that all validators will inherit from.
    internal abstract class ValidatorBase<TRecord, TDescriptor> where TRecord : ConfigTableRecord where TDescriptor : CustomBaseDescriptor
    {
        public abstract ImportOperationResult Validate(TRecord record, TDescriptor descriptor);
    }
}
