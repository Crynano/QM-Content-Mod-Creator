using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Helpers;
using QM_ImporterAPI.Services.Validation.Base;
using QM_ImporterAPI.Templates.Descriptors;

namespace QM_ImporterAPI.Services.Validation
{
    internal class AugmentationValidator : ItemRecordValidator<AugmentationRecord, CustomAugmentationDescriptor>
    {
        public override ImportOperationResult Validate(AugmentationRecord record, CustomAugmentationDescriptor descriptor)
        {
            // Runs ConfigTableRecord and ItemRecord validations first, then adds the augmentation ones.
            var result = base.Validate(record, descriptor);

            if (record.WoundSlotIds == null || record.WoundSlotIds.Count == 0)
            {
                return result.AddError($"Augmentation \"{record.Id}\" has no wound slots defined.");
            }

            foreach (var woundId in record.WoundSlotIds)
            {
                var isInGame = QuasimorphHelper.IsGameId(woundId, Data.WoundSlots);
                if (!isInGame)
                {
                    result.AddWarning($"Augmentation \"{record.Id}\" has wound slot with ID \"{woundId}\" that does not exist.");
                }
            }

            var isTooltipinGame = QuasimorphHelper.IsGameId(record.TooltipIconTag, Data.TooltipIcons);
            if (!isTooltipinGame)
            {
                result.AddWarning($"Augmentation \"{record.Id}\" has tooltip icon with ID \"{record.TooltipIconTag}\" that does not exist.");
            }

            return result;
        }
    }
}
