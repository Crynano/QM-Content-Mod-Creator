using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Templates.Descriptors;

namespace QM_ImporterAPI.Services.Validation.Base
{
    internal class BreakableItemRecordValidator<TRecord, TDescriptor> : ItemRecordValidator<TRecord, TDescriptor>
        where TRecord : BreakableItemRecord
        where TDescriptor : CustomItemContentDescriptor
    {
        public override ImportOperationResult Validate(TRecord record, TDescriptor descriptor)
        {
            var result = base.Validate(record, descriptor);

            if (record.MaxDurability < 1)
            {
                result.AddWarning($"Item '{record.Id}' has a MaxDurability of {record.MaxDurability}.");
            }
            if (record.RepairItemIds == null || record.RepairItemIds.Count == 0)
            {
                result.AddWarning($"Item '{record.Id}' has no RepairItemIds defined.");
            }

            return result;
        }
    }
}
