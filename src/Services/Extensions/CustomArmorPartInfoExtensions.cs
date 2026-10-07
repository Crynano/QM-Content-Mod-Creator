using MGSC;
using QM_ImporterAPI.Services.Images;
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
                Texture = TextureImporter.ImportFromFile(customPart.TextureIdOrPath)
            };
        }

        public static List<ArmorPartInfo> ToGameList(this CustomArmorPartInfo customPart)
        {
            return new List<ArmorPartInfo>
            {
                customPart.ToGame()
            };
        }
    }
}
