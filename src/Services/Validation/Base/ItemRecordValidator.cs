using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Helpers;
using QM_ImporterAPI.Templates.Descriptors;

namespace QM_ImporterAPI.Services.Validation.Base
{
    internal class ItemRecordValidator<TRecord, TDescriptor> : ConfigTableRecordValidator<TRecord, TDescriptor>
        where TRecord : ItemRecord
        where TDescriptor : CustomBaseDescriptor
    {
        public override ImportOperationResult Validate(TRecord record, TDescriptor descriptor)
        {
            var result = base.Validate(record, descriptor);

            if (record.InventoryWidthSize <= 0)
            {
                result.AddError($"Item {record.Id} has an inventory width size of {record.InventoryWidthSize}. Inventory width size should be greater than 0.");
            }
            else if (record.InventoryWidthSize > 2)
            {
                result.AddError($"Item {record.Id} has an inventory width size of {record.InventoryWidthSize}. Inventory width size should be less than or equal to 2.");
            }

            foreach (var disassemblyItemId in record.Disassembly)
            {
                if (!QuasimorphHelper.IsGameId(disassemblyItemId.ItemId))                
{
                    result.AddWarning($"Item {record.Id} has disassembly item with ID '{disassemblyItemId.ItemId}' does not exist.");
                }
                if (disassemblyItemId.Count <= 0)
                {
                    result.AddWarning($"Item {record.Id} has disassembly item with ID '{disassemblyItemId.ItemId}' with a count of {disassemblyItemId.Count}. Count should be greater than 0.");
                }
            }

            return result;
        }
    }
}
