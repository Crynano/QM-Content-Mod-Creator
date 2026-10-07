using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Validation.Base;
using QM_ImporterAPI.Templates.Descriptors;
using System.Linq;

namespace QM_ImporterAPI.Services.Validation
{
    internal class ResistValidator<TRecord, TDescriptor> : BreakableItemRecordValidator<TRecord, TDescriptor>
        where TRecord : ResistRecord
        where TDescriptor : CustomResistDescriptor
    {
        public override ImportOperationResult Validate(TRecord record, TDescriptor descriptor)
        {
            var result = base.Validate(record, descriptor);

            foreach (var damage in record.ResistSheet.Select(resist => resist.damage))
            {
                if (string.IsNullOrEmpty(damage))
                {
                    result.AddWarning($"Item '{record.Id}' has a resist entry with an empty damage type.");
                }
                else if (!Data.DamageTypes.Ids.Contains(damage))
                {
                    result.AddWarning($"Item '{record.Id}' has a resist entry with an invalid damage type '{damage}'.");
                }
            }

            return result;
        }
    }
}
