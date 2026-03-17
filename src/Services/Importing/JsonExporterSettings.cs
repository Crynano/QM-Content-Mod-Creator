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
            NullValueHandling = NullValueHandling.Include, // TODO Test if changing prints ItemClass
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Converters = new List<JsonConverter>()
            {
                new Newtonsoft.Json.Converters.StringEnumConverter()
            },
            ContractResolver = new WeaponContractResolver()
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

    public class WeaponContractResolver : DefaultContractResolver
    {
        private static readonly List<string> PropertiesNotToInclude = new List<string>()
        {
            "ItemClass", "ItemDesc", "ContentDesc", "ContentDescriptor"
        };

        public WeaponContractResolver()
        {
            
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

            // only serializer properties that start with the specified character
            properties = properties.Where(p => !IsInList(p.PropertyName)).ToList();

            return properties;
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