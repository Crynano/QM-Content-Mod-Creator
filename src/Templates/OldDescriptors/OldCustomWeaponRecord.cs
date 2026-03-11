using MGSC;
using System.Collections.Generic;

namespace QM_ImporterAPI.Templates.OldDescriptors
{
    public class OldCustomWeaponRecord
    {
        public string id { get; set; }
        public ItemClass? itemClass { get; set; }
        public int? techLevel { get; set; }
        public float? price { get; set; }
        public float? weight { get; set; }
        public int? inventoryWidthSize { get; set; }
        public List<string> categories { get; set; }
        public int? minimumDamage { get; set; }
        public int? maximumDamage { get; set; }
        public int? criticalChance { get; set; }
        public float? criticalDamage { get; set; }
        public int? range { get; set; }
        public int? magazineCapacity { get; set; }
        public int? reloadDuration { get; set; }
        public bool? reloadOneBulletAtATime { get; set; }
        public string defaultAmmoId { get; set; }
        public string requiredAmmo { get; set; }
        public List<string> overrideAmmo { get; set; }
        public bool? isSelfCharge { get; set; }
        public int? minRandomAmmoCount { get; set; }
        public List<string> firemodes { get; set; }
        public int? maxDurability { get; set; }
        public int? minDurabilityAfterRepair { get; set; }
        public bool? unbreakable { get; set; }
        public string repairCategory { get; set; }
        public List<string> repairItemIds { get; set; }
        public int? grip { get; set; }
        public WeaponClass? weaponClass { get; set; }
        public WeaponSubClass? weaponSubClass { get; set; }
        public string defaultGrenadeId { get; set; }
        public List<string> AllowedGrenadeIds { get; set; }
        public float? bonusAccuracy { get; set; }
        public float? bonusScatterAngle { get; set; }
        public float? falloff { get; set; }
        public float? silencerShotChance { get; set; }
        public float? armorPenetration { get; set; }
        public float? rangeThrowbackChanceBonus { get; set; }
        public bool? rangeExtraThrowback { get; set; }
        public float? critPainDamageMultiplier { get; set; }
        public float? offSlotCritChance { get; set; }
        public int? rampUpValue { get; set; }
        public int? obstaclePierceChanceBonus { get; set; }
        public float? creaturePierceBonus { get; set; }
        public float? woundChanceOnPierce { get; set; }
        public float? fovLookAngleMult { get; set; }
        public int? dotWoundsDamageBonus { get; set; }
        public int? fractureWoundDamageBonus { get; set; }
        public float? painDamageMultiplier { get; set; }
        public bool? amputationOnWound { get; set; }
        public bool? hasHFGOverlay { get; set; }
        public bool? isImplicit { get; set; }
        public string overrideProjectileId { get; set; }

        public List<string> traits { get; set; }
    }
}