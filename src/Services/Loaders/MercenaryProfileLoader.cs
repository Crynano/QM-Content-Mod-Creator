using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Services.Loaders
{
    /// <summary>
    /// Loader for mercenary profile records.
    /// </summary>
    internal class MercenaryProfileLoader : BaseItemLoader
    {
        protected override string LoaderName => nameof(MercenaryProfileLoader);

        public override ImportOperationResult Load(IEnumerable<object> deserializedObjects, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();

            var profileRecords = FilterByType<MercenaryProfileRecord>(deserializedObjects);

            LogLoadStart(profileRecords.Count(), 0);

            foreach (var profileRecord in profileRecords)
            {
                Logger.LogDebug($"Trying to add mercenary profile '{profileRecord.Id}' to the game!");
                var validationResult = ValidateMercProfile(profileRecord);
                if (validationResult.ErrorMessages.Any())
                {
                    operationResult.Absorb(validationResult);
                    continue;
                }

                var opResult = ItemCreator.CreateMercenaryProfile(profileRecord);
                operationResult.Absorb(opResult);
            }

            return operationResult;
        }

        private ImportOperationResult ValidateMercProfile(MercenaryProfileRecord mercProfile)
        {
            var operationResult = new ImportOperationResult();

            if (!MGSC.Data.Perks.Ids.Contains(mercProfile.TalentPerkId))
            {
                operationResult.AddError($"Perk '{mercProfile.TalentPerkId}' does not exist in-game");
            }

            foreach (var wound in mercProfile.WoundSlots.Where(wound => !MGSC.Data.WoundSlots.Ids.Contains(wound)))
            {
                operationResult.AddError($"WoundSlot '{wound}' does not exist in-game");
            }

            return operationResult;
        }
    }
}
