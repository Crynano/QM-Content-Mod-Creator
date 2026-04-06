using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QM_ImporterAPI.Services.Importing
{
    public static class JsonExporterSettings
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.None,
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Include,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Converters = new List<JsonConverter>()
            {
                new Newtonsoft.Json.Converters.StringEnumConverter()
            },
            ContractResolver = new RecordContractResolver()
        };

        private static readonly JsonSerializerSettings JsonDeserializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Include,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Converters = new List<JsonConverter>()
            {
                new Newtonsoft.Json.Converters.StringEnumConverter()
            },
            ContractResolver = new ProtectedSetterContractResolver()
        };

        public static JsonSerializerSettings SerializerSettings => JsonSerializerSettings;
        public static JsonSerializerSettings DeserializerSettings => JsonDeserializerSettings;
    }

    public class RecordContractResolver : DefaultContractResolver
    {
        private readonly List<string> PropertiesNotToInclude = new List<string>()
        {
            "ItemDesc", "ContentDesc", "ContentDescriptor", "FireModeView"
        };

        private readonly List<string> PropertyOrder = new List<string>()
        {
            "RecordType",
            "Data",
            "Id",
            "ItemId",
            "OutputItem",
            "ItemClass",
            "TechLevel",
            "Price",
            "Weight",
            "InventorySortOrder",
            "InventoryWidthSize",
            "Grip",
            "WeaponClass",
            "WeaponSubClass",
            "IsMelee",
            "Categories",
            "Damage",
            "Firemodes",
            "RequiredAmmo",
            "DefaultAmmoId",
            "OverrideAmmo",
            "OverrideProjectileId",
            "Range",
            "BonusAccuracy",
            "BonusScatterAngle",
            "Falloff",
            "ReloadDuration",
            "MagazineCapacity",
            "MinRandomAmmoCount",
            "MaxDurability",
            "MinDurabilityAfterRepair",
            "Unbreakable",
            "RepairItemIds",
            "Traits",
            "DefaultGrenadeId",
            "AllowedGrenadeIds",
            "CanDisassembly",
            "Disassembly",
            "DurabilityLossOnThrow",
            "ThrowRange",
            "MeleeCanAmputate",
            "GetMeleeDamageFromCreature",
            "DotWoundsDmgBonus",
            "FractureWoundDmgBonus",
            "CanPutInVest",
            "IsImplicit"
        };

        public RecordContractResolver()
        {

        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
            properties = properties.Where(p => !IsInList(p.PropertyName)).ToList();

            var orderedProperties = properties
                .OrderBy(p =>
                {
                    var index = PropertyOrder.IndexOf(p.PropertyName);
                    return index == -1 ? int.MaxValue : index;
                })
                .ThenBy(p => p.PropertyName)
                .ToList();

            return orderedProperties;
        }

        private bool IsInList(string word)
        {
            return PropertiesNotToInclude.Contains(word, StringComparer.OrdinalIgnoreCase);
        }
    }

    public class ProtectedSetterContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jProperty = base.CreateProperty(member, memberSerialization);
            if (jProperty.Writable)
                return jProperty;

            jProperty.Writable = member.IsPropertyWithSetter();

            return jProperty;
        }
    }
    internal static class MemberInfoExtensions
    {
        internal static bool IsPropertyWithSetter(this MemberInfo member)
        {
            var property = member as PropertyInfo;

            return property?.SetMethod != null;
        }
    }
}