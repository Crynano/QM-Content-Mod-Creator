using MGSC;
using QM_ImporterAPI.Templates.OldDescriptors;
using System.Collections.Generic;

namespace QM_ImporterAPI.Services.Mappers
{
    public static class WeaponRecordExtensions
    {
        public static WeaponRecord ToNew(this OldCustomWeaponRecord oldRecord)
        {
            return new WeaponRecord()
            {
                Id = oldRecord.id,
                ItemClass = oldRecord.itemClass ?? default,
                TechLevel = oldRecord.techLevel ?? default,
                Price = oldRecord.price ?? default,
                Weight = oldRecord.weight ?? default,
                InventoryWidthSize = oldRecord.inventoryWidthSize ?? default,
                Categories = oldRecord.categories ?? new List<string>(),
                Damage = new DmgInfo()
                {
                    minDmg = oldRecord.minimumDamage ?? default,
                    maxDmg = oldRecord.maximumDamage ?? default,
                    critChance = oldRecord.criticalChance ?? default,
                    critDmg = oldRecord.criticalDamage ?? default,
                },
                Range = oldRecord.range ?? default,
                MagazineCapacity = oldRecord.magazineCapacity ?? default,
                ReloadDuration = oldRecord.reloadDuration ?? default,
                DefaultAmmoId = oldRecord.defaultAmmoId,
                RequiredAmmo = oldRecord.requiredAmmo,
                OverrideAmmo = oldRecord.overrideAmmo ?? new List<string>(),
                MinRandomAmmoCount = oldRecord.minRandomAmmoCount ?? default,
                Firemodes = oldRecord.firemodes ?? new List<string>(),
                MaxDurability = oldRecord.maxDurability ?? default,
                MinDurabilityAfterRepair = oldRecord.minDurabilityAfterRepair ?? default,
                Unbreakable = oldRecord.unbreakable ?? default,
                RepairItemIds = oldRecord.repairItemIds ?? (string.IsNullOrEmpty(oldRecord.repairCategory) 
                    ? new List<string>() 
                    : new List<string> { oldRecord.repairCategory }),
                WeaponClass = oldRecord.weaponClass ?? default,
                WeaponSubClass = oldRecord.weaponSubClass ?? default,
                DefaultGrenadeId = oldRecord.defaultGrenadeId,
                AllowedGrenadeIds = oldRecord.AllowedGrenadeIds ?? new List<string>(),
                BonusAccuracy = oldRecord.bonusAccuracy ?? default,
                BonusScatterAngle = oldRecord.bonusScatterAngle ?? default,
                Falloff = oldRecord.falloff ?? default,
                IsImplicit = oldRecord.isImplicit ?? default,
                Traits = oldRecord.traits ?? new List<string>(),
                OverrideProjectileId = oldRecord.overrideProjectileId,
                DotWoundsDmgBonus = oldRecord.dotWoundsDamageBonus ?? default,
                FractureWoundDmgBonus = oldRecord.fractureWoundDamageBonus ?? default
            };
        }
    }
}