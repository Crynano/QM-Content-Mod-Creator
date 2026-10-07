using MGSC;
using QM_ImporterAPI.Templates.Descriptors;
using System.Collections.Generic;
using System.Linq;

namespace QM_ImporterAPI.Services.Extensions
{
    internal static class CustomArmorPartInfoExtensions
    {
        public static List<ArmorPartInfo> ToGame(this List<CustomArmorPartInfo> customParts)
        {
            return customParts.Select(part => part.ToGame()).ToList();
        }

        public static ArmorPartInfo ToGame(this CustomArmorPartInfo customPart)
        {
            return new ArmorPartInfo
            {
                ArmorType = customPart.ArmorType,
                ArmorPart = customPart.ArmorPart,
                Texture = null // TODO import texture from path or item
            };
        }
    }
}
